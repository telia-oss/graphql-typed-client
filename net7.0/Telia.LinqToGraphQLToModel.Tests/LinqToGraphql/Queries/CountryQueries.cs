using SystemLibrary.Common.Net;

using Telia.LinqToGraphQLToModel.Tests.LinqToGraphql.Schema;

namespace Telia.LinqToGraphQLToModel.Tests.LinqToGraphql.Queries;

public class CountryQueries : GraphQLQuery<CountrySchema>
{
    public string GetCountry(string countryCode)
    {
        var filter = new CountryFilter() { CountryCode = countryCode };

        return Query(q => q.CountryCollection(filter).Countries);
    }

    public string GetCountries()
    {
        return Query(q => q.CountryCollection().Countries);
    }

    public IEnumerable<Country> GetCountryCollectionWithCountriesAsResponse()
    {
        return Query(q => q.CountryCollection().Countries, GetCountryCollectionWithCountriesResponse);
    }

    public IEnumerable<Country> GetCountriesResponse()
    {
        return Query(q => q.Countries, GetCountriesResponse);
    }

    string GetCountriesResponse(string graphql)
    {
        return Assemblies.GetEmbeddedResource("GraphqlResponseToModels/Assets", "countries-response.txt");
    }

    string GetCountryCollectionWithCountriesResponse(string graphql)
    {
        return Assemblies.GetEmbeddedResource("GraphqlResponseToModels/Assets", "country-collection-with-countries-response.txt");
    }
}