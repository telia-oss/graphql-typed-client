using Newtonsoft.Json.Linq;

namespace Telia.LinqToGraphQL.Response
{
    public class GraphQLResponse
    {
        public JObject Data { get; set; }
        public IEnumerable<GraphQLError> Errors { get; set; }
    }

    public class GraphQLResponse<T>
    {
        public T Data { get; }
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
}
