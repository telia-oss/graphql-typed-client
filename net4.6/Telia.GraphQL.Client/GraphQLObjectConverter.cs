using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Telia.GraphQL.Client.Attributes;

namespace Telia.GraphQL.Client
{
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
            var jo = new JObject();

            foreach (PropertyInfo prop in type.GetProperties())
            {
                var fieldAttribute = prop.GetCustomAttribute<GraphQLFieldAttribute>();
                if (prop.CanRead && fieldAttribute != null)
                {
                    var propVal = prop.GetValue(value, null);
                    if (propVal != null)
                    {
                        jo.Add(fieldAttribute.Name, JToken.FromObject(propVal, serializer));
                    }
                }
            }
            jo.WriteTo(writer);
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
            var props = objectType.GetTypeInfo().DeclaredProperties.ToList();

            foreach (JProperty jp in jObject.Properties())
            {
                var prop = props.FirstOrDefault(pi =>
                    pi.CanWrite &&
                    pi.GetCustomAttribute<GraphQLFieldAttribute>().Name.ToLower() == jp.Name.ToLower());

                prop?.SetValue(instance, jp.Value.ToObject(prop.PropertyType, serializer));
            }
        }
    }
}
