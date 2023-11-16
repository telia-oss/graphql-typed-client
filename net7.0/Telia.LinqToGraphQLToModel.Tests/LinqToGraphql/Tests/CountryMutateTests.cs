using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Net;

using Telia.LinqToGraphQLToModel.Tests._Abstract;
using Telia.LinqToGraphQLToModel.Tests.LinqToGraphql.Queries;

namespace Telia.LinqToGraphQLToModel.Tests.LinqToGraphql.Tests;

[TestClass]
public class MutationTests : BaseTestClass
{
    [TestMethod]
    public void Mutate_Country_Returns_Id()
    {
        var countryCode = "SE";
        var name = "Sweden";
        var id = 694;

        var mutations = new CountryMutations();

        var query = mutations.CountryMutate(countryCode, name, id);

        var expected = GetExpected("Mutate_Country_Returns_Id");

        IsEqualIgnoreWhitespace(query, expected);
    }

    string GetExpected(string fileName)
    {
        return Assemblies.GetEmbeddedResource("LinqToGraphql/Assets", fileName + ".txt");
    }
}
