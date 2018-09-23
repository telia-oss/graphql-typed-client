using System.Collections.Generic;
using System.Linq.Expressions;

namespace GraphQLTypedClient.Client
{
    internal class CallChain
    {
        private readonly List<ChainLink> links;
        private readonly Expression node;

        public IEnumerable<ChainLink> Links => this.links;
        public Expression Node => this.node;

        public CallChain(List<ChainLink> links, Expression node)
        {
            this.links = links;
            this.node = node;
        }
    }
}
