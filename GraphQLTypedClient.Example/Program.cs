using System;

namespace GraphQLTypedClient.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new Client("https://3kvq1jpw0v.lp.gql.zone/graphql");

            var data = client.Query(e => new
            {
                name = e.Actor("1").Name
            });

            Console.Write(data.name);
        }
    }
}
