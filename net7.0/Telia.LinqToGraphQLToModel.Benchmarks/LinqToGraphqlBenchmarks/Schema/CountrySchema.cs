namespace Telia.LinqToGraphQL.Benchmarks.LinqToGraphqlBenchmarks.Schema;

using System;
using System.Collections.Generic;

using Telia.GraphQL.Schema.Attributes;

[GraphQLType("CountryFilter")]
public class CountryFilter
{
    [GraphQLField("countryCode", "String")]
    public virtual string CountryCode { get; set; }
}

[GraphQLType("Country")]
public class Country
{
    [GraphQLField("countryCode", "String")]
    public virtual string CountryCode { get; set; }

    [GraphQLField("price", "String")]
    public virtual string Price { get; set; }
}

[GraphQLType("CountryCollection")]
public class CountryCollection
{
    [GraphQLField("countries", "[Country]")]
    public virtual IEnumerable<Country> Countries { get; set; }
}

[GraphQLType("Query")]
public class CountrySchema
{
    [GraphQLField("countryCollection", "CountryCollection")]
    public virtual CountryCollection CountryCollection()
    {
        throw new InvalidOperationException();
    }

    [GraphQLField("countryCollection", "CountryCollection")]
    public virtual CountryCollection CountryCollection([GraphQLArgument("filter", "CountryFilter")] CountryFilter filter)
    {
        throw new InvalidOperationException();
    }
}
