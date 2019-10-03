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
                var path = "";
                var groupedLink = rootLinks;
				ChainLink lastLink = null;

                foreach (var part in chain.Links)
                {
					lastLink = this.TryGroup(part, groupedLink, ref path);
					groupedLink = lastLink.Children as List<ChainLink>;
                }

                if (lastLink != null)
                {
                    lastLink.Node = chain.Node;
                    this.context.AddBinding(chain.Node, path.Substring(1));
                }
			}

            return rootLinks;
        }

        private ChainLink TryGroup(ChainLink part, List<ChainLink> groupedLink, ref string path)
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

            path = $"{path}.field{groupedLink.IndexOf(existingLink)}";

            return existingLink;
        }
    }
}
