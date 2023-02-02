namespace Telia.GraphQL.Client.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Parameter)]
    public class GraphQLArgumentAttribute : Attribute
    {
        public GraphQLArgumentAttribute(string name, string graphQLType)
        {
            this.Name = name;
            this.GraphQLType = graphQLType;
        }

        public string Name { get; }
        public string GraphQLType { get; }
    }
}
