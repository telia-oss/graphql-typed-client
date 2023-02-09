using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Net;

using Telia.LinqToGraphQL.LinqToGraphql.Queries;
using Telia.LinqToGraphQLToModel.Tests._Abstract;

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

    string GetExpected(string fileName)
    {
        return Assemblies.GetEmbeddedResource("LinqToGraphql/Assets", fileName + ".txt");
    }
}
