namespace Mkm.GraphQL.Client.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public class GraphQLFieldAttribute : Attribute
    {
        public GraphQLFieldAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; }
    }
}
