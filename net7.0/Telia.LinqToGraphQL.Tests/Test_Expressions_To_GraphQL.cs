using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Net;

using Telia.GraphQL;
using Telia.GraphQL.Schema;

namespace Telia.LinqToGraphQL.Tests
{
    [TestClass]
    public class Test_Expressions_To_GraphQL
    {
        [TestMethod]
        public void Test()
        {
            var expected = Assemblies.GetEmbeddedResource("Assets", "expected-country-korea.json");
            var countryCode = "KR";

            var countryQueries = new CountryQueries();

            var query = countryQueries.GetCountryCode(countryCode);

            IsEqualIgnoreWhitespace(query, expected);
        }

        public class CountryQueries : GraphQLQuery<Query>
        {
            public string GetCountryCode(string countryCode)
            {
                var filter = new CountryFilter() { CountryCode = countryCode };

                return Query(x => x.CountriesCollection(filter).CountryCode);
            }
        }


        void IsEqualIgnoreWhitespace(string text, string expected)
        {
            expected = ClearWhiteSpaceAndNewLines(expected);

            text = ClearWhiteSpaceAndNewLines(text);

            Assert.IsTrue(text == expected);
        }

        string ClearWhiteSpaceAndNewLines(string data)
        {
            var sb = new StringBuilder(data);

            sb.Replace(Environment.NewLine, " ")
                .Replace("\t", " ")
                .Replace("\\r\\n", " ")
                .Replace("\\n", " ")
                .Replace("  ", " ")
                .Replace("  ", " ")
                .Replace("  ", " ")
                .Replace("  ", " ")
                .Replace(": ", ":")
                .Replace("{ ", "{")
                .Replace(" }", "}")
                .Replace(", ", ",");

            return sb.ToString();
        }
    }
}
