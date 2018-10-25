using GraphQLParser;
using Telia.GraphQL.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq.Expressions;

namespace Telia.GraphQL
{
    public abstract class GraphQLCLient<TQueryType>
    {
        protected readonly INetworkClient client;

        public GraphQLCLient(string endpoint) : this(new DefaultNetworkClient(endpoint))
        {

        }

        public GraphQLCLient(INetworkClient client)
        {
            this.client = client;
        }

        public TReturn Query<TReturn>(Expression<Func<TQueryType, TReturn>> selector)
        {
            var context = new QueryContext();

            var query = this.CreateQuery(selector, context);
            var composer = new ResponseComposer<TQueryType, TReturn>(selector, context);

            var response = JsonConvert.DeserializeObject<JObject>(this.client.Send(query));

            return composer.Compose(response);
        }

        public string CreateQuery<TReturn>(Expression<Func<TQueryType, TReturn>> selector)
        {
            return this.CreateQuery(selector, new QueryContext());
        }

        internal string CreateQuery<TType, TReturn>(
            Expression<Func<TType, TReturn>> selector,
            QueryContext context)
        {
            var astPrinter = new Printer();

            var grouping = new SelectionChainGrouping(context);
            var converter = new SelectionChainConverter();
            var visitor = new PathGatheringVisitor(context);

            visitor.Visit(selector);

            var groupedChains = grouping.Group();

            return astPrinter.Print(converter.Convert(groupedChains));
        }
    }

    public abstract class GraphQLCLient<TQueryType, TMutationType>: GraphQLCLient<TQueryType>
    {
        public GraphQLCLient(string endpoint) : base(endpoint)
        {
        }

        public GraphQLCLient(INetworkClient client) : base(client)
        {
        }

        public string CreateMutation<TReturn>(Expression<Func<TMutationType, TReturn>> selector)
        {
            return this.CreateQuery(selector, new QueryContext());
        }

        public TReturn Mutation<TReturn>(Expression<Func<TMutationType, TReturn>> selector)
        {
            var context = new QueryContext();

            var query = this.CreateQuery(selector, context);
            var composer = new ResponseComposer<TMutationType, TReturn>(selector, context);

            var response = JsonConvert.DeserializeObject<JObject>(this.client.Send(query));

            return composer.Compose(response);
        }
    }
}
