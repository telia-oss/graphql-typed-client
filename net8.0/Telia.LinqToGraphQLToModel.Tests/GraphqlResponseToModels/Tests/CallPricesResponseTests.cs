using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Extensions;

using Telia.LinqToGraphQLToModel.Tests._Abstract;
using Telia.LinqToGraphQLToModel.Tests.GraphqlResponseToModels.Queries;

namespace Telia.LinqToGraphQLToModel.Tests.GraphqlResponseToModels.Tests;

[TestClass]
public class CallPricesResponseTests : BaseTestClass
{
    [TestMethod]
    public void MultiWithNullAndErrors()
    {
        var queries = new CallPricesQueries();

        var callPriceModel = queries.GetCallPricesModel();

        Assert.IsTrue(callPriceModel != null);

        Assert.IsTrue(callPriceModel.OfferingPriceDetails.IsNot());
        Assert.IsTrue(callPriceModel.CallPrices.IsNot());

        Assert.IsTrue(callPriceModel.OtherCallPrices.First().MmsPrice > 0);
    }
}
