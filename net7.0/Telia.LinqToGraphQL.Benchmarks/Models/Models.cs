namespace Test;

using System;
using System.Collections.Generic;

using Telia.GraphQL;
using Telia.GraphQL.Schema.Attributes;

[GraphQLTypeAttribute("CountryFilter")]
public class CountryFilter
{
    [GraphQLFieldAttribute("countryCode", "String")]
    public virtual String CountryCode { get; set; }
}

[GraphQLTypeAttribute("Country")]
public class Country
{
    [GraphQLFieldAttribute("countryCode", "String")]
    public virtual String CountryCode { get; set; }

    [GraphQLFieldAttribute("price", "String")]
    public virtual String Price { get; set; }
}

[GraphQLTypeAttribute("CountryCollection")]
public class CountryCollection
{
    [GraphQLFieldAttribute("countries", "[Country]")]
    public virtual IEnumerable<Country> Countries { get; set; }
}

[GraphQLTypeAttribute("Query")]
public class Query
{
    [GraphQLFieldAttribute("countryCollection", "CountryCollection")]
    public virtual CountryCollection CountryCollection()
    {
        throw new InvalidOperationException();
    }

    public virtual CountryCollection CountryCollection([GraphQLArgumentAttribute("filter", "CountryFilter")] CountryFilter filter)
    {
        throw new InvalidOperationException();
    }
}

public class CountryQueries : GraphQLQuery<Query>
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