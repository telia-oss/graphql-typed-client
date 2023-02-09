using Telia.GraphQL;
using Telia.LinqToGraphQL.Benchmarks.LinqToGraphqlBenchmarks.Schema;

namespace Telia.LinqToGraphQL.Benchmarks.LinqToGraphqlBenchmarks.Queries;

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