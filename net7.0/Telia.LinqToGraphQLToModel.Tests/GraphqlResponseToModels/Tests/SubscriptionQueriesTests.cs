using Microsoft.VisualStudio.TestTools.UnitTesting;

using Telia.LinqToGraphQL.LinqToGraphql.Queries;
using Telia.LinqToGraphQLToModel.Tests._Abstract;
using Telia.LinqToGraphQLToModel.Tests.LinqToGraphqlToModels.Queries;

namespace Telia.LinqToGraphQLToModel.Tests.LinqToGraphqlToModels.Tests;

[TestClass]
public class SubscriptionQueriesTests : BaseTestClass
{
    [TestMethod]
    public void Query_Subscriptions_Responds_With_Exact_Properties_Converts_Successfully()
    {
        var queries = new SubscriptionQueries();

        var subscriptions = queries.Subscriptions();

        Assert.IsTrue(subscriptions != null, "subscriptions is null");

        Assert.IsTrue(subscriptions.Count() == 3);

        Assert.IsTrue(subscriptions.First().PhoneNumber.CountryCode == "47");
        Assert.IsTrue(subscriptions.First().PhoneNumber.LocalNumber.Is());
        Assert.IsTrue(subscriptions.First().LegalOwner.Individual.EmailAddress.Contains("@"));
        Assert.IsTrue(subscriptions.First().Roles.Count() == 3);
    }

    [TestMethod]
    public void Query_Subscriptions_Responds_With_Less_Properties_Converts_Successfully()
    {
        var queries = new SubscriptionQueries();

        var subscriptions = queries.SubscriptionsPartialResponse();

        Assert.IsTrue(subscriptions != null, "subscriptions is null");

        Assert.IsTrue(subscriptions.Count() == 3, "wrong count");

        Assert.IsTrue(subscriptions.First().PhoneNumber.CountryCode == "47", "Country is not 47");
        Assert.IsTrue(subscriptions.First().PhoneNumber.LocalNumber.IsNot());
        Assert.IsTrue(subscriptions.First().LegalOwner.Individual.EmailAddress.Contains("@"));
        Assert.IsTrue(subscriptions.First().Roles.Count() == 3);
    }
}
