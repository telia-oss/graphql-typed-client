using System.Reflection;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Telia.LinqToGraphQLToModel.Schema.Attributes;

namespace Telia.LinqToGraphQLToModel.Response;

public class GraphQLObjectConverter : JsonConverter
{
    public override bool CanWrite => true;

    public override bool CanRead => true;

    public override bool CanConvert(Type objectType)
    {
        return objectType.GetCustomAttribute<GraphQLTypeAttribute>() != null &&
               !objectType.IsInterface &&
                objectType.IsClass;
    }

    public override void WriteJson(JsonWriter writer,
        object value, JsonSerializer serializer)
    {
        var type = value.GetType();
        var obj = new JObject();

        foreach (PropertyInfo property in type.GetProperties())
        {
            var attribute = property.GetCustomAttribute<GraphQLFieldAttribute>();
            if (property.CanRead && attribute != null)
            {
                var propertyValue = property.GetValue(value, null);
                if (propertyValue != null)
                {
                    obj.Add(attribute.Name, JToken.FromObject(propertyValue, serializer));
                }
            }
        }

        obj.WriteTo(writer);
    }

    public override object ReadJson(JsonReader reader,
        Type objectType, object existingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return null;

        var instance = Activator.CreateInstance(objectType);
        var jObject = JObject.Load(reader);

        LoadFromJObject(objectType, jObject, instance, serializer);

        return instance;
    }

    protected void LoadFromJObject(Type objectType, JObject jObject, object instance, JsonSerializer serializer)
    {
        var properties = objectType.GetTypeInfo().DeclaredProperties.ToList();

        foreach (JProperty property in jObject.Properties())
        {
            var p = properties.FirstOrDefault(propertyInfo =>
                propertyInfo.CanWrite &&
                propertyInfo.GetCustomAttribute<GraphQLFieldAttribute>().Name.ToLower() == property.Name.ToLower());

            p?.SetValue(instance, property.Value.ToObject(p.PropertyType, serializer));
        }
    }
}