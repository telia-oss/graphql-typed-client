using System.Linq.Expressions;

using GraphQLParser.AST;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using Telia.LinqToGraphQLToModel.GraphQLParser;
using Telia.LinqToGraphQLToModel.Response;

namespace Telia.LinqToGraphQLToModel;

public class GraphQLQuery<TQueryRoot>
{
    /// <summary>
    /// Create a graphql query through linq/expressions
    /// </summary>
    /// <example>
    /// 
    /// // Assume schema:
    /// public class Country
    /// {
    ///     public string CountryCode { get;set; }
    /// }
    /// 
    /// [GraphQlTypeAttribute(name: "Query")]   //Root class must be type 'Query'
    /// public class Schema
    /// {
    ///     [GraphQlFieldAttribute(name: "Countries")]
    ///     public List&lt;Country&gt; Countries() 
    ///     {
    ///         throw new Exception("Do not invoke this, its name is read through the expression-tree");
    ///     }
    /// }
    /// 
    /// var query = new GraphQLQuery&gt;Schema&lt;();
    /// 
    /// var graphQl = query.Query(q => q.Countries);
    /// 
    /// // graphQl variable equals:
    /// // { query: "Query { Countries { CountryCode } } ", variables: { }
    /// </example>
    public string Query<TSelector>(Expression<Func<TQueryRoot, TSelector>> selector, JsonSerializerSettings querySerializerSettings = null)
    {
        return CreateOperation<TQueryRoot, TSelector, string>(selector, OperationType.Query, null, querySerializerSettings);
    }

    /// <summary>
    /// Create a graphql query that is passed into your callback function, the second parameter, which then returns the response 'as is', which this method will serialize into C# models
    /// </summary>
    /// <example>
    /// 
    /// // Assume schema:
    /// public class Country
    /// {
    ///     public string CountryCode { get;set; }
    /// }
    /// 
    /// [GraphQlTypeAttribute(name: "Query")]   //Root class must be type 'Query'
    /// public class Schema
    /// {
    ///     [GraphQlFieldAttribute(name: "Countries")]
    ///     public List&lt;Country&gt; Countries() 
    ///     {
    ///         throw new Exception("Do not invoke this, its name is read through the expression-tree");
    ///     }
    /// }
    /// 
    /// string OnSend(string graphql) {
    ///     // Pseudo code:
    ///     return new HttpClient(url, graphql).Response.Data;
    /// }
    /// 
    /// var query = new GraphQLQuery&gt;Schema&lt;();
    /// 
    /// var countries = query.Query(q => q.Countries, OnSend); // Uses the function OnSend, callback...
    /// </example>
    public TSelector Query<TSelector>(Expression<Func<TQueryRoot, TSelector>> selector, Func<string, string> onSendGraphQl, JsonSerializerSettings querySerializerSettings = null)
    {
        return CreateOperation<TQueryRoot, TSelector, TSelector>(selector, OperationType.Query, onSendGraphQl, querySerializerSettings);
    }

    internal TResult CreateOperation<TType, TSelector, TResult>(Expression<Func<TType, TSelector>> selector, OperationType operationType, Func<string, string> onSendGraphQl = null, JsonSerializerSettings querySerializerSettings = null)
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

        var temp = new GraphQLQueryData(query, variableValues);

        string graphqlQuery;
        if(querySerializerSettings == null)
        {
            graphqlQuery = JsonConvert.SerializeObject(temp, new JsonSerializerSettings()
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
            graphqlQuery = JsonConvert.SerializeObject(temp, querySerializerSettings);
        }

        if(onSendGraphQl == null)
        {
            return (TResult)(object)graphqlQuery;
        }

        var composer = new ResponseComposer<TType, TSelector>(selector, context);

        var response = onSendGraphQl(graphqlQuery);

        if (response.IsNot()) return default;
        
        var graphQlResponse = JsonConvert.DeserializeObject<GraphQLResponse>(response);

        if (graphQlResponse?.Data == null) return default;

        return (TResult)(object)composer.Compose(graphQlResponse.Data);
    }
}
