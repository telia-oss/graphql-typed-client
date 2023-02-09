namespace Telia.GraphQL.Schema.Attributes;

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
