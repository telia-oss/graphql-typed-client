using System.Linq.Expressions;
using System.Reflection;

using GraphQLParser;
using GraphQLParser.AST;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

using Telia.GraphQL.Client;
using Telia.GraphQL.Client.JsonConverters;
using Telia.GraphQL.Schema.Attributes;

namespace Telia.GraphQL;

public abstract class GraphQLQuery<TQueryRoot>
{
    /// <summary>
    /// After invoking Query() or Mutate() this variable contains a reference to the last resulting Query() or Mutate() invocation
    /// </summary>
    public GraphQLQueryData Data { get; private set; }

    public virtual string Query<TReturn>(Expression<Func<TQueryRoot, TReturn>> selector)
    {
        return this.CreateOperation(selector, OperationType.Query);
    }

    internal string CreateOperation<TType, TReturn>(Expression<Func<TType, TReturn>> selector, OperationType operationType)
    {
        var context = new QueryContext();
        var grouping = new SelectionChainGrouping(context);
        var converter = new SelectionChainConverter(context);
        var visitor = new PathGatheringVisitor(context);
        var expander = new SelectionChainExpander(context);

        visitor.Visit(selector);

        var expanded = expander.Expand();

        context.SelectionChains.AddRange(expanded);

        var groupedChains = grouping.Group();

        var variableDefinitions = new GraphQLVariablesDefinition();
        variableDefinitions.Items = new List<GraphQLVariableDefinition>();
        var variableValues = new Dictionary<string, object>();

        var printer = new Printer();

        var operationDefinition = new GraphQLOperationDefinition
        {
            Name = operationType.ToString().ToGraphQlName(),
            Operation = operationType,
            Variables = variableDefinitions,
            SelectionSet = converter.Convert(groupedChains, variableDefinitions, variableValues)
        };

        var query = printer.Print(operationDefinition);

        Data = new GraphQLQueryData(query, variableValues);

        if (GraphQLJsonConverters.JsonConverters == null)
        {
            return JsonConvert.SerializeObject(Data, new JsonSerializerSettings()
            {
                Converters = new List<JsonConverter>()
                {
                    new GraphQLObjectConverter(),
                    new StringEnumConverter()
                }
            });
        }
        else
        {
            return JsonConvert.SerializeObject(Data, new JsonSerializerSettings()
            {
                Converters = GraphQLJsonConverters.JsonConverters
            });
        }
    }
}

internal class GraphQLObjectConverter : JsonConverter
{
    public override bool CanWrite => true;

    public override bool CanRead => true;

    public override bool CanConvert(Type objectType)
    {
        if (objectType.GetCustomAttribute<GraphQLTypeAttribute>() != null && !objectType.IsInterface)
        {
            return objectType.IsClass;
        }

        return false;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        Type type = value.GetType();
        JObject jObject = new JObject();
        PropertyInfo[] properties = type.GetProperties();
        foreach (PropertyInfo propertyInfo in properties)
        {
            GraphQLFieldAttribute customAttribute = propertyInfo.GetCustomAttribute<GraphQLFieldAttribute>();
            if (propertyInfo.CanRead && customAttribute != null)
            {
                object value2 = propertyInfo.GetValue(value, null);
                if (value2 != null)
                {
                    jObject.Add(customAttribute.Name, JToken.FromObject(value2, serializer));
                }
            }
        }

        jObject.WriteTo(writer);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return null;
        }

        object obj = Activator.CreateInstance(objectType);
        JObject jObject = JObject.Load(reader);
        LoadFromJObject(objectType, jObject, obj, serializer);
        return obj;
    }

    protected void LoadFromJObject(Type objectType, JObject jObject, object instance, JsonSerializer serializer)
    {
        List<PropertyInfo> source = objectType.GetTypeInfo().DeclaredProperties.ToList();
        foreach (JProperty jp in jObject.Properties())
        {
            PropertyInfo propertyInfo = source.FirstOrDefault((PropertyInfo pi) => pi.CanWrite && pi.GetCustomAttribute<GraphQLFieldAttribute>().Name.ToLower() == jp.Name.ToLower());
            propertyInfo?.SetValue(instance, jp.Value.ToObject(propertyInfo.PropertyType, serializer));
        }
    }
}
