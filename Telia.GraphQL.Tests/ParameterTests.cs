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

        [Test]
        public void Query_DateTimeParameter_CreatesCorrectQuery()
        {
            var networkClient = Substitute.For<INetworkClient>();

            var client = new TestClient(networkClient);

            var dateTime = DateTime.Parse("2008-09-22T14:01:54.9571247Z").ToUniversalTime();

            var query = client.CreateQuery(e => new
            {
                test = e.DateTimeParam(dateTime)
            });

            AssertUtils.AreEqualIgnoreLineBreaks(@"{
  field0: test(dt: ""2008-09-22T14:01:54Z"")
  __typename
}", query);
        }

        [Test]
        public void Query_InputType_CreatesCorrectQuery()
        {
            var networkClient = Substitute.For<INetworkClient>();

            var client = new TestClient(networkClient);

            var query = client.CreateQuery(e => new
            {
                test = e.InputObj(new SomeInputObject { 
                    Faz = 42,
                    Bar = null
                })
            });

            AssertUtils.AreEqualIgnoreLineBreaks(@"{
  field0: test(input: {faz: 42, bar: null})
  __typename
}", query);
        }

        [Test]
        public void Query_ArrayInputType_CreatesCorrectQuery()
        {
            var networkClient = Substitute.For<INetworkClient>();

            var client = new TestClient(networkClient);

            var query = client.CreateQuery(e => new
            {
                test = e.ArrayOfInputObj(new[]
                {
                    new SomeInputObject { Faz = 42, Bar = null },
                    new SomeInputObject { Faz = 12, Bar = "test" }
                })
            });

            AssertUtils.AreEqualIgnoreLineBreaks(@"{
  field0: test(input: [{faz: 42, bar: null}, {faz: 12, bar: ""test""}])
  __typename
}", query);
        }

        private class TestQuery
        {
            [GraphQLField("test")]
            public int ArrayAsParameter(IEnumerable<int> arr) { throw new InvalidOperationException(); }

            [GraphQLField("test")]
            public int NullableParam(int? arr) { throw new InvalidOperationException(); }

            [GraphQLField("test")]
            public int DateTimeParam(DateTime dt) { throw new InvalidOperationException(); }

            [GraphQLField("test")]
            public int InputObj(SomeInputObject input) { throw new InvalidOperationException(); }

            [GraphQLField("test")]
            public int ArrayOfInputObj(IEnumerable<SomeInputObject> input) { throw new InvalidOperationException(); }
        }

        [GraphQLType("SomeInputObject")]
        public class SomeInputObject
        {
            [GraphQLField("foo")]
            public virtual String Foo
            {
                get;
                set;
            }

            [GraphQLField("bar")]
            public virtual String Bar
            {
                get;
                set;
            }

            [GraphQLField("faz")]
            public virtual Int32? Faz
            {
                get;
                set;
            }
        }

        private class TestClient : GraphQLCLient<TestQuery>
        {
            public TestClient(INetworkClient client) : base(client)
            {
            }
        }
    }
}
