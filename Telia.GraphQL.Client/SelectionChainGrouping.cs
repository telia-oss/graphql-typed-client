using System.Collections.Generic;
using System.Linq;

namespace Telia.GraphQL.Client
{
	internal class SelectionChainGrouping
    {
		private readonly QueryContext context;

		public SelectionChainGrouping(QueryContext context)
		{
			this.context = context;
		}

		public IEnumerable<ChainLink> Group()
        {
            var rootLinks = new List<ChainLink>();

            foreach (var chain in this.context.SelectionChains)
            {
                var groupedLink = rootLinks;
				ChainLink lastLink = null;

                foreach (var part in chain.Links)
                {
					lastLink = this.TryGroup(part, groupedLink);
					groupedLink = lastLink.Children as List<ChainLink>;
                }

                lastLink?.Nodes.Add(chain.Node);
            }

            return rootLinks;
        }

        private ChainLink TryGroup(ChainLink part, List<ChainLink> groupedLink)
        {
            var existingLink = groupedLink.SingleOrDefault(e => e.Equals(part));

            if (existingLink == null)
            {
                existingLink = new ChainLink(
                    part.FieldName,
                    part.UseAlias,
                    part.Arguments,
                    new List<ChainLink>());

                existingLink.Fragment = part.Fragment;

                groupedLink.Add(existingLink);
            }
            
            return existingLink;
        }
    }
}
