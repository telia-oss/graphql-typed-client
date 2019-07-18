using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Telia.GraphQL.Client
{
    public class GraphQLResult
    {
        public JObject Data { get; set; }
        public IEnumerable<GraphQLError> Errors { get; set; }
    }

    public class GraphQLResult<T>
    {
        public T Data { get; }
        public IEnumerable<GraphQLError> Errors { get; }

        public GraphQLResult(T value, IEnumerable<GraphQLError> errors)
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
