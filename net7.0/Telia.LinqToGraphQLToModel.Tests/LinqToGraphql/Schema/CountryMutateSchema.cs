using Telia.LinqToGraphQLToModel.Schema.Attributes;

namespace Telia.LinqToGraphQLToModel.Tests.LinqToGraphql.Schema;

[GraphQLType("Mutation")]
public class CountryMutateSchema
{
    [GraphQLFieldAttribute("Country", "Country")]
    public virtual CountryMutateResponse Country([GraphQLArgumentAttribute("input", "CountryMutateInput")] CountryMutateInput input)
    {
        throw new InvalidOperationException();
    }
}

[GraphQLTypeAttribute("CountryMutateResponse")]
public class CountryMutateResponse
{
    [GraphQLFieldAttribute("id", "String")]
    public virtual String Id { get; set; }
}

[GraphQLTypeAttribute("CountryMutateInput")]
public class CountryMutateInput
{
    [GraphQLFieldAttribute("id", "Int")]
    public virtual int Id { get; set; }

    [GraphQLFieldAttribute("name", "String!")]
    public virtual String Name { get; set; }
    [GraphQLFieldAttribute("countryCode", "String!")]
    public virtual String CountryCode { get; set; }
}
