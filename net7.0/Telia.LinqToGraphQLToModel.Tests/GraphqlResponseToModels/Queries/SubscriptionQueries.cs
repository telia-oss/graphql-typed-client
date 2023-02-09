using SystemLibrary.Common.Net;

using Telia.LinqToGraphQLToModel.Tests.GraphqlResponseToModels.Schema;

namespace Telia.LinqToGraphQLToModel.Tests.GraphqlResponseToModels.Queries;

public class SubscriptionQueries : GraphQLQuery<TeliaSchema>
{
    public string GetFullResponse(string graphql)
    {
        return Assemblies.GetEmbeddedResource("GraphqlResponseToModels/Assets", "subscription-response-full.txt");
    }

    public string GetResponseMissingLastPropertyLocalNumber(string graphql)
    {
        // Last property of an object is missing, "localNumber", should still work
        return Assemblies.GetEmbeddedResource("GraphqlResponseToModels/Assets", "subscription-response-missing-last-property-localnumber.txt");
    }

    public IEnumerable<SubscriptionData> Subscriptions()
    {
        return Query(q => q.Subscriptions()
            .Select(sub => new SubscriptionData()
            {
                AgreementType = sub.AgreementType,
                PhoneNumber = sub.PhoneNumber,
                ProductUser = new ProductUser
                {
                    FirstName = sub.ProductUser.FirstName,
                    Surname = sub.ProductUser.Surname,
                    EmailAddress = sub.ProductUser.EmailAddress,
                    TelephoneNumber = new PhoneNumber
                    {
                        CountryCode = sub.ProductUser.TelephoneNumber.CountryCode,
                        LocalNumber = sub.ProductUser.TelephoneNumber.LocalNumber
                    }
                },
                LegalOwner = new Customer
                {
                    Individual = new ProductUser
                    {
                        FirstName = sub.LegalOwner.Individual.FirstName,
                        Surname = sub.LegalOwner.Individual.Surname,
                        EmailAddress = sub.LegalOwner.Individual.EmailAddress,
                        TelephoneNumber = sub.LegalOwner.Individual.TelephoneNumber
                    }
                },
                Roles = sub.Roles,
                SubscriptionStatusDate = sub.SubscriptionStatusDate,
                SubscriptionRank = sub.SubscriptionRank,
                SubscriptionType = sub.SubscriptionType,
                CommitmentStartDate = sub.CommitmentStartDate,
                CommitmentEndDate = sub.CommitmentEndDate,
            }), GetFullResponse);
    }

    public IEnumerable<SubscriptionData> SubscriptionsPartialResponse()
    {
        return Query(q => q.Subscriptions()
            .Select(sub => new SubscriptionData()
            {
                AgreementType = sub.AgreementType,
                PhoneNumber = sub.PhoneNumber,
                ProductUser = new ProductUser
                {
                    FirstName = sub.ProductUser.FirstName,
                    Surname = sub.ProductUser.Surname,
                    EmailAddress = sub.ProductUser.EmailAddress,
                    TelephoneNumber = new PhoneNumber
                    {
                        CountryCode = sub.ProductUser.TelephoneNumber.CountryCode,
                        LocalNumber = sub.ProductUser.TelephoneNumber.LocalNumber
                    }
                },
                LegalOwner = new Customer
                {
                    Individual = new ProductUser
                    {
                        FirstName = sub.LegalOwner.Individual.FirstName,
                        Surname = sub.LegalOwner.Individual.Surname,
                        EmailAddress = sub.LegalOwner.Individual.EmailAddress,
                        TelephoneNumber = sub.LegalOwner.Individual.TelephoneNumber
                    }
                },
                Roles = sub.Roles,
                SubscriptionStatusDate = sub.SubscriptionStatusDate,
                SubscriptionRank = sub.SubscriptionRank,
                SubscriptionType = sub.SubscriptionType,
                CommitmentStartDate = sub.CommitmentStartDate,
                CommitmentEndDate = sub.CommitmentEndDate,
            }), GetResponseMissingLastPropertyLocalNumber);
    }
}
