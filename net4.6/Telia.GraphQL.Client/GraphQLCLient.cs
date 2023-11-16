using GraphQLParser;
using Telia.GraphQL.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using GraphQLParser.AST;

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

        public virtual GraphQLResult<TReturn> Query<TReturn>(Expression<Func<TQueryType, TReturn>> selector)
        {
            var context = new QueryContext();

            var query = this.CreateOperation(selector, context, OperationType.Query);
            var composer = new ResponseComposer<TQueryType, TReturn>(selector, context);
            var result = this.client.Send(query);

            if (string.IsNullOrWhiteSpace(result))
            {
                return new GraphQLResult<TReturn>(default, null);
            }

            var response = JsonConvert.DeserializeObject<GraphQLResult>(result);
            var value = response.Data == null
                ? default
                : composer.Compose(response.Data);

            return new GraphQLResult<TReturn>(value, response.Errors);
        }

        public virtual GraphQLQueryInfo CreateQuery<TReturn>(Expression<Func<TQueryType, TReturn>> selector)
        {
            return this.CreateOperation(selector, new QueryContext(), OperationType.Query);
        }

        internal GraphQLQueryInfo CreateOperation<TType, TReturn>(
            Expression<Func<TType, TReturn>> selector,
            QueryContext context,
            OperationType operationType)
        {
            var astPrinter = new Printer();

            var grouping = new SelectionChainGrouping(context);
            var converter = new SelectionChainConverter(context);
            var visitor = new PathGatheringVisitor(context);
            var expander = new SelectionChainExpander(context);

            visitor.Visit(selector);
            var expanded = expander.Expand();

            context.SelectionChains.AddRange(expanded);

            var groupedChains = grouping.Group();
            var variableDefinitions = new List<GraphQLVariableDefinition>();
            var variableValues = new Dictionary<string, object>();

            var query = astPrinter.Print(new GraphQLOperationDefinition
            {
                Name = new GraphQLName()
                {
                    Value = operationType.ToString()
                },
                VariableDefinitions = variableDefinitions,
                Operation = operationType,
                SelectionSet = converter.Convert(groupedChains, variableDefinitions, variableValues)
            });

            return new GraphQLQueryInfo(query, variableValues);
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

        public virtual GraphQLQueryInfo CreateMutation<TReturn>(Expression<Func<TMutationType, TReturn>> selector)
        {
            return this.CreateOperation(selector, new QueryContext(), OperationType.Mutation);
        }

        public virtual GraphQLResult<TReturn> Mutation<TReturn>(Expression<Func<TMutationType, TReturn>> selector)
        {
            var context = new QueryContext();

            var query = this.CreateOperation(selector, context, OperationType.Mutation);
            var composer = new ResponseComposer<TMutationType, TReturn>(selector, context);
            var result = this.client.Send(query);

            if (string.IsNullOrWhiteSpace(result))
            {
                return new GraphQLResult<TReturn>(default, null);
            }

            var response = JsonConvert.DeserializeObject<GraphQLResult>(result);
            var value = response.Data == null
                ? default
                : composer.Compose(response.Data);

            return new GraphQLResult<TReturn>(value, response.Errors);
        }
    }
}
