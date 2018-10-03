namespace Telia.GraphQL.Client.Example
{
	public class GraphQLClient : GraphQLCLient<Query>
	{
		public GraphQLClient(string endpoint) : base(endpoint)
		{
		}
	}
}
