// NOTE: Just a sample of dummy objects to fake a real case scenario

using Telia.LinqToGraphQLToModel.Schema.Attributes;

namespace Telia.LinqToGraphQLToModel.Tests.GraphqlResponseToModels.Schema;

[GraphQLTypeAttribute("AgreementType")]
public enum AgreementType
{
    BUSINESS,
    PRIVATE
}

[GraphQLTypeAttribute("AccountData")]
public class AccountData
{
    [GraphQLFieldAttribute("id", "String!")]
    public virtual String Id { get; set; }

    [GraphQLFieldAttribute("agreementType", "AgreementType!")]
    public virtual AgreementType AgreementType { get; set; }

    [GraphQLFieldAttribute("roles", "[UserRole!]!")]
    public virtual IEnumerable<UserRole> Roles { get; set; }

    [GraphQLFieldAttribute("subscription", "SubscriptionData!")]
    public virtual SubscriptionData Subscription([GraphQLArgumentAttribute("phoneNumber", "String!")] String phoneNumber)
    {
        throw new InvalidOperationException();
    }

    [GraphQLFieldAttribute("subscriptions", "[SubscriptionData!]")]
    public virtual IEnumerable<SubscriptionData> Subscriptions { get; set; }
    
    [GraphQLFieldAttribute("legalOwner", "Customer")]
    public virtual Customer LegalOwner { get; set; }
}

[GraphQLTypeAttribute("Customer")]
public class Customer
{
    [GraphQLFieldAttribute("individual", "ProductUser")]
    public virtual ProductUser Individual { get; set; }
}

[GraphQLTypeAttribute("Country")]
public class Country
{
    [GraphQLFieldAttribute("code", "String!")]
    public virtual String Code { get; set; }

    [GraphQLFieldAttribute("name", "String!")]
    public virtual String Name { get; set; }
}

[GraphQLTypeAttribute("PhoneNumber")]
public class PhoneNumber
{
    [GraphQLFieldAttribute("countryCode", "String!")]
    public virtual String CountryCode { get; set; }

    [GraphQLFieldAttribute("localNumber", "String!")]
    public virtual String LocalNumber { get; set; }
}

[GraphQLTypeAttribute("ProductUser")]
public class ProductUser
{
    [GraphQLFieldAttribute("title", "String")]
    public virtual String Title { get; set; }

    [GraphQLFieldAttribute("firstName", "String")]
    public virtual String FirstName { get; set; }

    [GraphQLFieldAttribute("surname", "String")]
    public virtual String Surname { get; set; }

    [GraphQLFieldAttribute("emailAddress", "String")]
    public virtual String EmailAddress { get; set; }

    [GraphQLFieldAttribute("telephoneNumber", "PhoneNumber")]
    public virtual PhoneNumber TelephoneNumber { get; set; }

    [GraphQLFieldAttribute("birthDate", "AWSDate")]
    public virtual DateTime? BirthDate { get; set; }
}

[GraphQLTypeAttribute("SubscriptionData")]
public class SubscriptionData
{
    [GraphQLFieldAttribute("subscriptionType", "String")]
    public virtual String SubscriptionType { get; set; }

    [GraphQLFieldAttribute("account", "AccountData!")]
    public virtual AccountData Account { get; set; }

    [GraphQLFieldAttribute("phoneNumber", "PhoneNumber!")]
    public virtual PhoneNumber PhoneNumber { get; set; }

    [GraphQLFieldAttribute("legalOwner", "Customer")]
    public virtual Customer LegalOwner { get; set; }

    [GraphQLFieldAttribute("productUser", "ProductUser")]
    public virtual ProductUser ProductUser { get; set; }

    [GraphQLFieldAttribute("agreementType", "AgreementType")]
    public virtual AgreementType? AgreementType { get; set; }

    [GraphQLFieldAttribute("subscriptionStatusDate", "AWSDate")]
    public virtual DateTime? SubscriptionStatusDate { get; set; }

    [GraphQLFieldAttribute("commitmentStartDate", "AWSDate")]
    public virtual DateTime? CommitmentStartDate { get; set; }

    [GraphQLFieldAttribute("commitmentEndDate", "AWSDate")]
    public virtual DateTime? CommitmentEndDate { get; set; }

    [GraphQLFieldAttribute("roles", "[UserRole!]!")]
    public virtual IEnumerable<UserRole> Roles { get; set; }

    [GraphQLFieldAttribute("subscriptionRank", "Int")]
    public virtual Int32? SubscriptionRank { get; set; }
}

[GraphQLTypeAttribute("UserRole")]
public enum UserRole
{
    COMPANY,
    LEGAL_OWNER,
    INVOICE_PAYER,
    PRODUCT_USER,
    USER_OF_AGE,
    UNKNOWN
}

[GraphQLTypeAttribute("UserData")]
public class UserData
{
    [GraphQLFieldAttribute("id", "ID!")]
    public virtual String Id { get; set; }

    [GraphQLFieldAttribute("roles", "[UserRole!]")]
    public virtual IEnumerable<UserRole> Roles { get; set; }

    [GraphQLFieldAttribute("phoneNumber", "PhoneNumber")]
    public virtual PhoneNumber PhoneNumber { get; set; }

    [GraphQLFieldAttribute("isAccountOwner", "Boolean")]
    public virtual Boolean? IsAccountOwner { get; set; }
}

[GraphQLTypeAttribute("Query")]
public class TeliaSchema
{
    [GraphQLFieldAttribute("subscription", "SubscriptionData!")]
    public virtual SubscriptionData Subscription()
    {
        throw new InvalidOperationException();
    }

    [GraphQLFieldAttribute("subscription", "SubscriptionData!")]
    public virtual SubscriptionData Subscription([GraphQLArgumentAttribute("phoneNumber", "String")] String phoneNumber)
    {
        throw new InvalidOperationException();
    }

    [GraphQLFieldAttribute("subscription", "SubscriptionData!")]
    public virtual SubscriptionData Subscription([GraphQLArgumentAttribute("phoneNumber", "String")] String phoneNumber, [GraphQLArgumentAttribute("noCache", "Boolean")] Boolean? noCache)
    {
        throw new InvalidOperationException();
    }

    [GraphQLFieldAttribute("subscriptions", "[SubscriptionData!]")]
    public virtual IEnumerable<SubscriptionData> Subscriptions()
    {
        throw new InvalidOperationException();
    }

    [GraphQLFieldAttribute("subscriptions", "[SubscriptionData!]")]
    public virtual IEnumerable<SubscriptionData> Subscriptions([GraphQLArgumentAttribute("noCache", "Boolean")] Boolean? noCache)
    {
        throw new InvalidOperationException();
    }

    [GraphQLFieldAttribute("account", "AccountData!")]
    public virtual AccountData Account()
    {
        throw new InvalidOperationException();
    }

    [GraphQLFieldAttribute("account", "AccountData!")]
    public virtual AccountData Account([GraphQLArgumentAttribute("accountId", "String")] String accountId)
    {
        throw new InvalidOperationException();
    }

    [GraphQLFieldAttribute("accounts", "[AccountData!]")]
    public virtual IEnumerable<AccountData> Accounts { get; set; }

    [GraphQLFieldAttribute("user", "UserData")]
    public virtual UserData User { get; set; }
}

[GraphQLTypeAttribute("CountryFilter")]
public class CountryFilter
{
    [GraphQLFieldAttribute("countryCode", "String")]
    public virtual String CountryCode { get; set; }
}
