﻿using Telia.LinqToGraphQLToModel.Schema.Attributes;

namespace Telia.LinqToGraphQLToModel.Tests.LinqToGraphql.Schema;

[GraphQLType("CountryFilter")]
public class CountryFilter
{
    [GraphQLField("countryCode", "String")]
    public virtual string CountryCode { get; set; }
}

[GraphQLType("Operator")]
public class Operator
{
    [GraphQLField("name", "String")]
    public virtual String Name { get; set; }

    [GraphQLField("price", "String")]
    public virtual String Price { get; set; }
}

[GraphQLType("Country")]
public class Country
{
    [GraphQLField("countryCode", "String")]
    public virtual string CountryCode { get; set; }

    [GraphQLField("name", "String")]
    public virtual string Name { get; set; }

    [GraphQLField("operators", "[Operator]")]
    public virtual IEnumerable<Operator> Operators { get; set; }
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

    [GraphQLField("countries", "[Country]")]
    public virtual IEnumerable<Country> Countries { get; set; }
}


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