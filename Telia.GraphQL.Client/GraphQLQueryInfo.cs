using System.Collections.Generic;
using Newtonsoft.Json;

namespace Telia.GraphQL.Client
{
    public class GraphQLQueryInfo
    {
        [JsonProperty("query")]
        public string Query { get; }

        [JsonProperty("variables")]
        public IDictionary<string, object> Variables { get; }

        public GraphQLQueryInfo(string query, IDictionary<string, object> variables)
        {
            Query = query;
            Variables = variables;
        }
    }
}
