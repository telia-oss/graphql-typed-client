﻿namespace Telia.GraphQL.Client.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Enum)]
    public class GraphQLTypeAttribute : Attribute
    {
        public GraphQLTypeAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; }
    }
}
