using Telia.GraphQL.Client.Attributes;
using NUnit.Framework;
using System.Collections.Generic;
using NSubstitute;
using System.Linq;
using System;

namespace Telia.GraphQL.Tests
{
	[TestFixture]
    public class ParameterTests
	{
        [Test]
        public void Query_ArrayAsParameter_CreatesCorrectQuery()
        {
			var networkClient = Substitute.For<INetworkClient>();
			networkClient.Send(Arg.Any<string>()).Returns("{ field0: 1 }");

			var client = new TestClient(networkClient);
            var param = new int[] { 1, 2, 3 };

            var query = client.CreateQuery(e => new
            {
                test = e.ArrayAsParameter(param)
            });

            Assert.AreEqual(@"{
  field0: test(arr: [1, 2, 3])
}", query);
        }

        [Test]
        public void Query_ListAsParameter_CreatesCorrectQuery()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<string>()).Returns("{ field0: 1 }");

            var client = new TestClient(networkClient);
            var param = new List<int> { 1, 2, 3 };

            var query = client.CreateQuery(e => new
            {
                test = e.ArrayAsParameter(param)
            });

            Assert.AreEqual(@"{
  field0: test(arr: [1, 2, 3])
}", query);
        }

        private class TestQuery
        {
            [GraphQLField("test")]
            public int ArrayAsParameter(IEnumerable<int> arr) { throw new InvalidOperationException(); }
		}

        private class TestClient : GraphQLCLient<TestQuery>
        {
            public TestClient(INetworkClient client) : base(client)
            {
            }
        }
    }
}
