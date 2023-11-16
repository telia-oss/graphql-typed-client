using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Net;

using Telia.LinqToGraphQLToModel.Tests._Abstract;
using Telia.LinqToGraphQLToModel.Tests.LinqToGraphql.Queries;
using Telia.LinqToGraphQLToModel.Tests.LinqToGraphql.Schema;

namespace Telia.LinqToGraphQLToModel.Tests.LinqToGraphql.Tests;

[TestClass]
public class LinqToGraphqlTests : BaseTestClass
{
    [TestMethod]
    public void Select_Countries_With_Filter_Returns_GraphQl()
    {
        var countryCode = "KR";

        var queries = new CountryQueries();

        var query = queries.GetCountry(countryCode);

        var expected = GetExpected("Select_Countries_With_Filter_Returns_GraphQl");

        IsEqualIgnoreWhitespace(query, expected);
    }

    [TestMethod]
    public void Select_Countries_Without_Filter_Returns_GraphQl()
    {
        var queries = new CountryQueries();

        var query = queries.GetCountries();

        var expected = GetExpected("Select_Countries_Without_Filter_Returns_GraphQl");

        IsEqualIgnoreWhitespace(query, expected);
    }


    [TestMethod]
    public void ReadMe_Schema_Query_Sample()
    {
        var schema = new GraphQLQuery<ReadMeSchema>();

        var graphQlQuery = schema.Query((e) => new
        {
            a = e.A,
            b = e.B(100)
        });

        var expected = "{\"query\":\"query Query($var_0: Int) { field0: a field1: b(x: $var_0) __typename}\",\"variables\":{\"var_0\":100}}";

        IsEqualIgnoreWhitespace(graphQlQuery, expected);
    }

    string GetExpected(string fileName)
    {
        return Assemblies.GetEmbeddedResource("LinqToGraphql/Assets", fileName + ".txt");
    }
}
