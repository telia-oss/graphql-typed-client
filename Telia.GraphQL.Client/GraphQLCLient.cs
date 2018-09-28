using GraphQLParser;
using Telia.GraphQL.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Telia.GraphQL
{
    public abstract class GraphQLCLient<TQueryType>
    {
        private readonly INetworkClient client;

        public GraphQLCLient(string endpoint) : this(new DefaultNetworkClient(endpoint))
        {

        }

        public GraphQLCLient(INetworkClient client)
        {
            this.client = client;
        }

        public TReturn Query<TReturn>(Expression<Func<TQueryType, TReturn>> selector)
        {
            var composer = new ResponseComposer();
            var bindings = new Dictionary<Expression, string>();

            var query = this.CreateQuery(selector, bindings);

            var response = JsonConvert.DeserializeObject<JObject>(this.client.Send(query));

            return composer.Compose(
                selector,
                response,
                bindings);
        }

        public string CreateQuery<TReturn>(Expression<Func<TQueryType, TReturn>> selector, IDictionary<Expression, string> bindings = null)
        {
            var astPrinter = new Printer();

            var grouping = new SelectionChainGrouping();
            var converter = new SelectionChainConverter();
            var visitor = new PathGatheringVisitor();

            visitor.Visit(selector);

            var groupedChains = grouping.Group(visitor.GetChains(), bindings);

            return astPrinter.Print(converter.Convert(groupedChains));
        }
    }
}
