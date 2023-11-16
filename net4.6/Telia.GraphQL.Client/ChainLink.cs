using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Telia.GraphQL.Client
{
    internal class ChainLink
    {
        public string FieldName { get; }
        public bool UseAlias { get; }
        public string Fragment { get; set; }
        public IEnumerable<ChainLinkArgument> Arguments { get; }
        public IEnumerable<ChainLink> Children { get; set; }
		public List<Expression> Nodes { get; internal set; }

		public ChainLink(string fieldName, bool useAlias, IEnumerable<ChainLinkArgument> arguments = null)
        {
            this.FieldName = fieldName;
            this.Arguments = arguments;
            this.UseAlias = useAlias;
            this.Nodes = new List<Expression>();
        }

        public ChainLink(
            string fieldName,
            bool useAlias,
            IEnumerable<ChainLinkArgument> arguments,
            IEnumerable<ChainLink> children): this(fieldName, useAlias, arguments)
        {
            this.Children = children;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ChainLink))
            {
                return false;
            }

            var link = obj as ChainLink;

            if (link.FieldName != this.FieldName)
            {
                return false;
            }

            if (link.Arguments?.Count() != this.Arguments?.Count())
            {
                return false;
            }

            if (link.Fragment != this.Fragment)
            {
                return false;
            }

            for (var i = 0; i < this.Arguments?.Count(); i++)
            {
                if (!link.Arguments.ElementAt(i).Equals(this.Arguments.ElementAt(i)))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
