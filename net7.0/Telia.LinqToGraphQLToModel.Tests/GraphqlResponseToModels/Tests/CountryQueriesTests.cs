using Microsoft.VisualStudio.TestTools.UnitTesting;

using Telia.LinqToGraphQL.LinqToGraphql.Queries;
using Telia.LinqToGraphQLToModel.Tests._Abstract;

namespace Telia.LinqToGraphQLToModel.Tests.LinqToGraphqlToModels.Tests;

[TestClass]
public class CountryQueriesTests : BaseTestClass
{
    [TestMethod]
    public void Query_Countries()
    {
        var queries = new CountryQueries();

        var countries = queries.GetCountriesResponse();

        Assert.IsTrue(countries != null, "countries is null");

        Assert.IsTrue(countries.Count() == 6, "countries wrong count " + countries.Count());

        Assert.IsTrue(countries.First().Operators.Count() == 0);
        Assert.IsTrue(countries.First().Name == "Diego Garcia");

        Assert.IsTrue(countries.First(x => x.Name != "Diego Garcia").Operators == null);

        Assert.IsTrue(countries.Last().Name == "Frankrike");
        Assert.IsTrue(countries.Last().Operators.Count() == 3);
        Assert.IsTrue(countries.Last().Operators.First().Name == "Yellow1");
        Assert.IsTrue(countries.Last().Operators.Last().Name == "Yellow3");
    }

    [TestMethod]
    public void Query_Country_Collection_WithCountries()
    {
        var queries = new CountryQueries();

        var countries = queries.GetCountryCollectionWithCountriesAsResponse();

        Assert.IsTrue(countries != null, "countries is null");

        Assert.IsTrue(countries.Count() == 6, "countries wrong count " + countries.Count());

        Assert.IsTrue(countries.First().Operators.Count() == 0);
        Assert.IsTrue(countries.First().Name == "Diego Garcia");

        Assert.IsTrue(countries.First(x => x.Name != "Diego Garcia").Operators == null);

        Assert.IsTrue(countries.Last().Name == "Frankrike");
        Assert.IsTrue(countries.Last().Operators.Count() == 3);
        Assert.IsTrue(countries.Last().Operators.First().Name == "Yellow1");
        Assert.IsTrue(countries.Last().Operators.Last().Name == "Yellow3");
    }
}
