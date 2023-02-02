using GraphQLParser.AST;
using Newtonsoft.Json;
using System;

namespace Telia.GraphQL.Client
{
    internal class ChainLinkArgument
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public string GraphQLType { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is ChainLinkArgument))
            {
                return false;
            }

            var arg = obj as ChainLinkArgument;

            return arg.Name == this.Name && this.ValuesAreTheSame(arg);
        }

        bool ValuesAreTheSame(ChainLinkArgument arg)
        {
            return JsonConvert.SerializeObject(arg.Value) == JsonConvert.SerializeObject(this.Value);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
