using Newtonsoft.Json.Linq;

namespace Telia.LinqToGraphQLToModel.Response;

public class GraphQLResponse
{
    public JObject Data { get; set; }
    public IEnumerable<GraphQLError> Errors { get; set; }
}

public class GraphQLResponse<T>
{
    public T Data { get; }

    //NOTE: Errors are just "there", parsed from the response, but they are ignored as of now,
    // Real exceptions on the protocol is up to the callee to respond to
    // Will do something with actual graphql error responses eventually...
    public IEnumerable<GraphQLError> Errors { get; }

    public GraphQLResponse(T value, IEnumerable<GraphQLError> errors)
    {
        Data = value;
        Errors = errors;
    }
}

public class GraphQLError
{
    public string Message { get; set; }
    public IEnumerable<object> Path { get; set; }
    public IEnumerable<GraphQLExceptionLocation> Locations { get; set; }
}

public class GraphQLExceptionLocation
{
    public int Line { get; set; }
    public int Column { get; set; }
}
