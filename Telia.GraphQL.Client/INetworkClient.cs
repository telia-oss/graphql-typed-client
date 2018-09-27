namespace Telia.GraphQL
{
    public interface INetworkClient
    {
        string Send(string query);
    }
}
