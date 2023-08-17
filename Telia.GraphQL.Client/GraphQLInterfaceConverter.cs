using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Telia.GraphQL.Client.Attributes;

namespace Telia.GraphQL.Client
{
    public class GraphQLInterfaceConverter : GraphQLObjectConverter
    {
        private static readonly IDictionary<string, IDictionary<string, Type>> typeBindings = new ConcurrentDictionary<string, IDictionary<string, Type>>();

        public override bool CanWrite => false;

        public override bool CanRead => true;

        private readonly Type queryType;

        public GraphQLInterfaceConverter(Type queryType)
        {
            EnsureCachedTypes(queryType);

            this.queryType = queryType;
        }

        private static void EnsureCachedTypes(Type queryType)
        {
            var fullQueryName = queryType.FullName;
            lock (typeBindings)
            {
                if (typeBindings.ContainsKey(fullQueryName))
                {
                    return;
                }

                var queryTypeDictionary = new Dictionary<string, Type>();
                var typesWithGraphQLTypeAttribute =
                    from t in queryType.Assembly.GetTypes()
                    let attribute = t.GetCustomAttribute<GraphQLTypeAttribute>(false)
                    where attribute != null
                    select new { Type = t, Attribute = attribute };

                foreach (var typeWithAttribute in typesWithGraphQLTypeAttribute)
                {
                    if (!queryTypeDictionary.ContainsKey(typeWithAttribute.Attribute.Name))
                    {
                        queryTypeDictionary.Add(typeWithAttribute.Attribute.Name, typeWithAttribute.Type);
                    }
                }

                typeBindings.Add(fullQueryName, queryTypeDictionary);
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.GetCustomAttribute<GraphQLTypeAttribute>() != null && objectType.IsInterface;
        }

        public override void WriteJson(JsonWriter writer,
            object value, JsonSerializer serializer)
        {
            throw new InvalidOperationException("Use default serialization.");
        }

        public override object ReadJson(JsonReader reader,
            Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (!typeBindings.ContainsKey(queryType.FullName))
            {
                return null;
            }

            if (reader.TokenType == JsonToken.Null) return null;

            var queryTypeCache = typeBindings[queryType.FullName];
            var jsonObject = JObject.Load(reader);
            var typeName = jsonObject["__typename"]?.ToString();

            if (string.IsNullOrWhiteSpace(typeName) || !queryTypeCache.ContainsKey(typeName))
            {
                return null;
            }

            var dotnetType = queryTypeCache[typeName];
            var instance = Activator.CreateInstance(dotnetType);

            LoadFromJObject(dotnetType, jsonObject, instance, serializer);

            return instance;
        }
    }
}
