using Telia.GraphQL.Client;

namespace Telia.GraphQL
{
    public interface INetworkClient
    {
        string Send(GraphQLQueryInfo query);
    }
}
