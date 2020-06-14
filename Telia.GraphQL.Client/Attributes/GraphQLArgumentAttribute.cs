namespace Telia.GraphQL.Client.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Parameter)]
    public class GraphQLArgumentAttribute : Attribute
    {
        public GraphQLArgumentAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; }
    }
}
