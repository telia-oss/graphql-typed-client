using Telia.GraphQL.Client.Attributes;
using NUnit.Framework;
using System.Collections.Generic;
using NSubstitute;
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

            AssertUtils.AreEqualIgnoreLineBreaks(@"{
  field0: test(arr: [1, 2, 3])
  __typename
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

            AssertUtils.AreEqualIgnoreLineBreaks(@"{
  field0: test(arr: [1, 2, 3])
  __typename
}", query);
        }

        [Test]
        public void Query_NonNullToNullParameter_CreatesCorrectQuery()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<string>()).Returns("{ field0: 1 }");

            var client = new TestClient(networkClient);

            var query = client.CreateQuery(e => new
            {
                test = e.NullableParam(1)
            });

            AssertUtils.AreEqualIgnoreLineBreaks(@"{
  field0: test(arr: 1)
  __typename
}", query);
        }

        private class TestQuery
        {
            [GraphQLField("test")]
            public int ArrayAsParameter(IEnumerable<int> arr) { throw new InvalidOperationException(); }

            [GraphQLField("test")]
            public int NullableParam(int? arr) { throw new InvalidOperationException(); }
        }

        private class TestClient : GraphQLCLient<TestQuery>
        {
            public TestClient(INetworkClient client) : base(client)
            {
            }
        }
    }
}
