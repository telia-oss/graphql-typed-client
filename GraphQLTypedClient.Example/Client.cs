using GraphQLTypedClient.Example.Schema;

namespace GraphQLTypedClient.Example
{
    public class Client : GraphQLCLient<Query>
    {
        public Client(string endpoint) : base(endpoint)
        {
        }
    }
}
