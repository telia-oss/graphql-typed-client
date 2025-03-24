using SystemLibrary.Common.Framework;

using Telia.LinqToGraphQLToModel.Tests.LinqToGraphql.Schema;

namespace Telia.LinqToGraphQLToModel.Tests.LinqToGraphql.Queries;

public class CountryMutations : GraphQLQuery<CountryMutateSchema>
{
    public string CountryMutate(string countryCode, string name, int id)
    {
        var update = new CountryMutateInput() { CountryCode = countryCode, Id = id, Name = name };

        return Mutate(m => m.Country(update));
    }

    string CountryMutateResponse(string graphql)
    {
        return Assemblies.GetEmbeddedResource("GraphqlResponseToModels/Assets/country-mutate-response.txt");
    }
}