using System.Collections.Generic;
using System.Linq.Expressions;

namespace Telia.GraphQL.Client
{
    internal class CallChain
    {
        private readonly List<ChainLink> links;
        private readonly Expression node;
        private readonly bool isTerminal;

        public IEnumerable<ChainLink> Links => this.links;
        public Expression Node => this.node;
        public bool IsTerminal => this.isTerminal;

        public CallChain(List<ChainLink> links, Expression node, bool isTerminal)
        {
            this.links = links;
            this.node = node;
            this.isTerminal = isTerminal;
        }
    }
}
