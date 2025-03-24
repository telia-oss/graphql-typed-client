using Telia.LinqToGraphQLToModel.Schema.Attributes;

namespace Telia.LinqToGraphQLToModel.Tests.LinqToGraphql.Schema;

[GraphQLType("Query")]
public class ReadMeSchema
{
    [GraphQLField("a", "Int")]
    public int? A { get; set; }

    [GraphQLField("b", "Int")]
    public int B([GraphQLArgument("x", "Int")] int x)
    {
        throw new InvalidOperationException();
    }
}