namespace Telia.GraphQLSchemaToCSharp.Tests.Attributes;

public class GraphQLTypeAttribute : Attribute
{
    public GraphQLTypeAttribute(string name, string graphQLType = null)
    {
    }
}

public class GraphQLFieldAttribute : Attribute
{
    public GraphQLFieldAttribute(string name, string graphQLType = null)
    {
    }
}

public class GraphQLArgumentAttribute : Attribute
{
    public GraphQLArgumentAttribute(string name, string type = null)
    {
    }
}

