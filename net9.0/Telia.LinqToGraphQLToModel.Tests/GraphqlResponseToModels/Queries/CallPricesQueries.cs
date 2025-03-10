using SystemLibrary.Common.Net;

using Telia.LinqToGraphQLToModel.Tests.LinqToGraphql.Schema;

namespace Telia.LinqToGraphQLToModel.Tests.GraphqlResponseToModels.Queries;

public class CallPricesQueries : GraphQLQuery<CallPricesSchema>
{
    string GetMultipleWithNullAndErrors(string graphql)
    {
        return Assemblies.GetEmbeddedResource("GraphqlResponseToModels/Assets", "multiple-selections-with-null-and-errors.txt");
    }

    public CallPricesModel GetCallPricesModel()
    {
        var countryCode = "DK";
        var countryFilter = new CountryFilter() { CountryCode = countryCode };
        var callPricesFilter = new CallPricesFilter { CountryCode = countryCode };

        var msisdn = "12344555";

        return Query(q => new CallPricesModel
        {
            OfferingPriceDetails = q.SubscriptionData(msisdn).UserOffering().OfferingPrices.Select(e => e.OfferingPrices()).FirstOrDefault(),
            CallPrices = q.SubscriptionData(msisdn).CallPricesCollection(callPricesFilter).CallPrices,
            OtherCallPrices = q.CallPricesCollection(callPricesFilter).CallPrices,
            AgreementType = q.SubscriptionData(msisdn).Account.AgreementType,
            Code = q.SubscriptionData(msisdn).UserOffering().PricePlan,

        }, GetMultipleWithNullAndErrors);
    }

}