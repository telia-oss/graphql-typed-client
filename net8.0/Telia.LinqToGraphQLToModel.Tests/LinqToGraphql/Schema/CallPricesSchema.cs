using Telia.LinqToGraphQLToModel.Schema.Attributes;
using Telia.LinqToGraphQLToModel.Tests.GraphqlResponseToModels.Schema;

namespace Telia.LinqToGraphQLToModel.Tests.LinqToGraphql.Schema;

[GraphQLType("Query")]
public class CallPricesSchema
{
    [GraphQLField("callPricesCollection", "CallPricesCollection")]
    public virtual CallPricesCollection CallPricesCollection([GraphQLArgumentAttribute("filter", "CallPricesFilter")] CallPricesFilter filter)
    {
        throw new InvalidOperationException();
    }

    [GraphQLField("callPrices", "[CallPrices]")]
    public virtual IEnumerable<CallPrices> CallPrices { get; set; }


    [GraphQLFieldAttribute("subscriptionData", "SubscriptionData!")]
    public virtual SubscriptionData SubscriptionData([GraphQLArgumentAttribute("phoneNumber", "String")] String phoneNumber)
    {
        throw new InvalidOperationException();
    }

    [GraphQLFieldAttribute("internationalPrices", "[CallPrices]")]
    public virtual IEnumerable<CallPrices> InternationalPrices { get; set; }
}

public class CallPricesModel
{
    public IEnumerable<OfferingPriceDetails> OfferingPriceDetails { get; set; }
    public IEnumerable<CallPrices> CallPrices { get; set; }
    public IEnumerable<CallPrices> OtherCallPrices { get; set; }
    public AgreementType AgreementType { get; set; }
    public string Code { get; set; }
}

[GraphQLTypeAttribute("CallPrices")]
public class CallPrices
{
    [GraphQLFieldAttribute("countryCode", "String")]
    public virtual String CountryCode { get; set; }

    [GraphQLFieldAttribute("smsPrice", "Float")]
    public virtual Single? SmsPrice { get; set; }

    [GraphQLFieldAttribute("mmsPrice", "Float")]
    public virtual Single? MmsPrice { get; set; }

    [GraphQLFieldAttribute("productOfferingDescription", "String")]
    public virtual String ProductOfferingDescription { get; set; }
}


[GraphQLType("CountryCollection")]
public class CallPricesCollection
{
    [GraphQLField("callPrices", "[CallPrices]")]
    public virtual IEnumerable<CallPrices> CallPrices { get; set; }
}

[GraphQLTypeAttribute("CallPricesFilter")]
public class CallPricesFilter
{
    [GraphQLFieldAttribute("countryCode", "String")]
    public virtual String CountryCode { get; set; }

    [GraphQLFieldAttribute("priceplan", "String")]
    public virtual String Priceplan { get; set; }
}


[GraphQLTypeAttribute("SubscriptionData")]
public class SubscriptionData
{
    [GraphQLFieldAttribute("subscriptionType", "String")]
    public virtual String SubscriptionType { get; set; }

    [GraphQLFieldAttribute("account", "AccountData!")]
    public virtual AccountData Account { get; set; }

    [GraphQLFieldAttribute("invoiceType", "String!")]
    public virtual String InvoiceType { get; set; }

    [GraphQLFieldAttribute("agreementType", "AgreementType")]
    public virtual AgreementType? AgreementType { get; set; }
    
    [GraphQLFieldAttribute("callPricesCollection", "CallPricesCollection")]
    public virtual CallPricesCollection CallPricesCollection()
    {
        throw new InvalidOperationException();
    }

    [GraphQLFieldAttribute("callPricesCollection", "CallPricesCollection")]
    public virtual CallPricesCollection CallPricesCollection([GraphQLArgumentAttribute("filter", "CallPricesFilter")] CallPricesFilter filter)
    {
        throw new InvalidOperationException();
    }

    [GraphQLFieldAttribute("userOffering", "UserOffering")]
    public virtual UserOffering UserOffering()
    {
        throw new InvalidOperationException();
    }
}


[GraphQLTypeAttribute("UserOffering")]
public class UserOffering
{
    [GraphQLFieldAttribute("pricePlan", "String")]
    public virtual String PricePlan { get; set; }

    [GraphQLFieldAttribute("name", "String")]
    public virtual String Name { get; set; }

    [GraphQLFieldAttribute("offeringPrices", "[OfferingPrice]")]
    public virtual IEnumerable<OfferingPrice> OfferingPrices { get; set; }

}


[GraphQLTypeAttribute("OfferingPrice")]
public class OfferingPrice
{
    [GraphQLFieldAttribute("offeringPrices", "[OfferingPriceDetails]")]
    public virtual IEnumerable<OfferingPriceDetails> OfferingPrices()
    {
        throw new InvalidOperationException();
    }
}



[GraphQLTypeAttribute("OfferingPriceDetails")]
public class OfferingPriceDetails
{
    [GraphQLFieldAttribute("code", "String")]
    public virtual String Code { get; set; }

    [GraphQLFieldAttribute("amount", "Float")]
    public virtual Single? Amount { get; set; }
}