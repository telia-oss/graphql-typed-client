using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GraphQLTypedClient.Client
{
    internal class SelectionChainGrouping
    {
        public IEnumerable<ChainLink> Group(IEnumerable<CallChain> chains, IDictionary<Expression, string> bindings)
        {
            var rootLinks = new List<ChainLink>();

            foreach (var chain in chains)
            {
                var path = "";
                var groupedLink = rootLinks;

                foreach (var part in chain.Links)
                {
                    groupedLink = this.TryGroup(part, groupedLink, ref path);
                }

                if (bindings != null)
                {
                    bindings.Add(chain.Node, path.Substring(1));
                }
            }

            return rootLinks;
        }

        private List<ChainLink> TryGroup(ChainLink part, List<ChainLink> groupedLink, ref string path)
        {
            var existingLink = groupedLink.SingleOrDefault(e => e.Equals(part));

            if (existingLink == null)
            {
                existingLink = new ChainLink(
                    part.FieldName,
                    part.Arguments,
                    new List<ChainLink>());

                groupedLink.Add(existingLink);
            }

            path = $"{path}.field{groupedLink.Count - 1}";

            return existingLink.Children as List<ChainLink>;
        }
    }
}
