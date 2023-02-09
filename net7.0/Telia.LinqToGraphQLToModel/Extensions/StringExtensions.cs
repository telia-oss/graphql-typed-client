using GraphQLParser.AST;

namespace Telia.LinqToGraphQLToModel;

internal static class StringExtensions
{
    public static ReadOnlyMemory<Char> ToCharMemory(this string value)
    {
        if (value == null) return new ReadOnlyMemory<char>();

        return new ReadOnlyMemory<char>(value.ToCharArray());
    }

    public static GraphQLName ToGraphQlName(this string value)
    {
        if (value.IsNot()) return null;

        return new GraphQLName(value.ToCharMemory());
    }
}
