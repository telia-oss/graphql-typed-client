using System;

namespace Telia.GraphQL.Playground
{
    public class SwapiClient : GraphQLCLient<Root>
    {
        public SwapiClient() : base("https://swapi-graphql.netlify.app/.netlify/functions/index")
        {
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var client = new SwapiClient();

            var startDateUtc = DateTime.Today.ToUniversalTime();
            var endDateUtc = startDateUtc.AddDays(1);

            var result = client.Query(schema => schema.AllStarships(null, 3));

            Console.ReadKey();
        }
    }
}
