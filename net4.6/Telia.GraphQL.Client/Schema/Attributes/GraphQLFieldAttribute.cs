﻿namespace Telia.GraphQL.Client.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public class GraphQLFieldAttribute : Attribute
    {
        public GraphQLFieldAttribute(string name, string graphQLType)
        {
            this.Name = name;
            this.GraphQLType = graphQLType;
        }

        public string Name { get; }
        public string GraphQLType { get; }
    }
}
