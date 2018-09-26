namespace GraphQLTypedClient
{
    public interface INetworkClient
    {
        string Send(string query);
    }
}
