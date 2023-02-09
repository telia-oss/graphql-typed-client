using Newtonsoft.Json;

namespace Telia.LinqToGraphQLToModel;

public class GraphQLQueryData
{
    [JsonProperty("query")]
    public string Query { get; }

    [JsonProperty("variables")]
    public IDictionary<string, object> Variables { get; }

    public GraphQLQueryData(string query, IDictionary<string, object> variables)
    {
        Query = query;
        Variables = variables;
    }
}
