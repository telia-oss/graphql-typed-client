using GraphQLParser.AST;
using System;

namespace GraphQLTypedClient.Client
{
    internal class ChainLinkArgument
    {
        public string Name { get; set; }
        public object Value { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is ChainLinkArgument))
            {
                return false;
            }

            var arg = obj as ChainLinkArgument;

            return arg.Name == this.Name && arg.Value == this.Value;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
