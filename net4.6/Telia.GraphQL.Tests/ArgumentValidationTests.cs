using System;
using NSubstitute;
using NUnit.Framework;
using Telia.GraphQL.Client;
using Telia.GraphQL.Client.Attributes;

namespace Telia.GraphQL.Tests
{
    [TestFixture]
    public class ArgumentValidationTests
    {
        [Test]
        public void Query_InputFailingOnNull_ThrowsAppropriateError()
        {
            var networkClient = Substitute.For<INetworkClient>();
            var client = new TestClient(networkClient);

            SomeInputObject input = null;

            var exception = Assert.Throws<ArgumentEvaluationException>(() => client.Query(e => e.Test(input.Test)));

            Assert.AreEqual("Evaluating argument \"input\" failed. See inner exception for details.", exception.Message);
            Assert.AreEqual("Object reference not set to an instance of an object.", exception.InnerException.Message);
        }

        private class SomeInputObject
        {
            public string Test { get; set; }
        }

        private class TestQuery
        {
            [GraphQLField("test", "Int!")]
            public int Test([GraphQLArgument("input", "String!")] string input) { throw new InvalidOperationException(); }
        }
        
        private class TestClient : GraphQLCLient<TestQuery>
        {
            public TestClient(INetworkClient client) : base(client)
            {
            }
        }
    }
}
