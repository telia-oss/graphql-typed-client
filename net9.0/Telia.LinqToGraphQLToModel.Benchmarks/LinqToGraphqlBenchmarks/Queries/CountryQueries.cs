using Telia.LinqToGraphQLToModel.Benchmarks.LinqToGraphqlBenchmarks.Schema;

namespace Telia.LinqToGraphQLToModel.Benchmarks.LinqToGraphqlBenchmarks.Queries;

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
}