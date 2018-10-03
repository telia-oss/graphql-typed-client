using Newtonsoft.Json;
using System;
using System.Linq;

namespace Telia.GraphQL.Client.Example
{
	class Program
	{
		static void Main(string[] args)
		{
			var client = new GraphQLClient("https://7rl74rlv5j.lp.gql.zone/graphql");

			var result = client.Query(e => new {
				empireHeroName = e.Hero(Episode.EMPIRE).Name,
				firendsOfLuke = e.Human("1000").Friends.Select(x => new {
					x.Name,
					theirFriends = x.Friends.Select(y => new {
						y.Name,
						theirFriends = y.Friends.Select(z => new {
							z.Name,
							theirFriends = x.Friends.Select(c => new {
								c.Name
							})
						})
					}),
					someOb = new
					{
						starships = x.Starships.Select(s => new { s.Id })
					}
				})
			});

			Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
			Console.ReadKey();
		}
	}
}
