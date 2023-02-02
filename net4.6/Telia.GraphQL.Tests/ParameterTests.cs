using System;
using System.Collections.Generic;

using NSubstitute;

using NUnit.Framework;

using Telia.GraphQL.Client;
using Telia.GraphQL.Client.Attributes;

namespace Telia.GraphQL.Tests
{
    [TestFixture]
    public class ParameterTests
	{
        [Test]
        public void Query_ArrayAsParameter_CreatesCorrectQuery()
        {
//			var networkClient = Substitute.For<DefaultNetworkClient();

//			networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns("{ field0: 1 }");

//			var client = new TestClient(networkClient);
//            var param = new int[] { 1, 2, 3 };

//            var query = client.CreateQuery(e => new
//            {
//                test = e.ArrayAsParameter(param)
//            });

//            AssertUtils.AreEqualIgnoreLineBreaks(@"query Query($var_0: [Int!]!) {
//  field0: test(arr: $var_0)
//  __typename
//}", query.Query);

//            Assert.AreEqual(param, query.Variables["var_0"]);
        }

        [Test]
        public void Query_ListAsParameter_CreatesCorrectQuery()
        {
            var networkClient = Substitute.For<DefaultNetworkClient>();
            networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns("{ field0: 1 }");

            var client = new TestClient(networkClient);
            var param = new List<int> { 1, 2, 3 };

            var query = client.CreateQuery(e => new
            {
                test = e.ArrayAsParameter(param)
            });

            AssertUtils.AreEqualIgnoreLineBreaks(@"query Query($var_0: [Int!]!) {
  field0: test(arr: $var_0)
  __typename
}", query.Query);

            Assert.AreEqual(param, query.Variables["var_0"]);
        }

        [Test]
        public void Query_NonNullToNullParameter_CreatesCorrectQuery()
        {
            var networkClient = Substitute.For<DefaultNetworkClient>();
            networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns("{ field0: 1 }");

            var client = new TestClient(networkClient);

            var query = client.CreateQuery(e => new
            {
                test = e.NullableParam(1)
            });

            AssertUtils.AreEqualIgnoreLineBreaks(@"query Query($var_0: Int) {
  field0: test(arr: $var_0)
  __typename
}", query.Query);

            Assert.AreEqual(1, query.Variables["var_0"]);
        }

        [Test]
        public void Query_DateTimeParameter_CreatesCorrectQuery()
        {
            var networkClient = Substitute.For<DefaultNetworkClient>();

            var client = new TestClient(networkClient);

            var dateTime = DateTime.Parse("2008-09-22T14:01:54.9571247Z").ToUniversalTime();

            var query = client.CreateQuery(e => new
            {
                test = e.DateTimeParam(dateTime)
            });

            AssertUtils.AreEqualIgnoreLineBreaks(@"query Query($var_0: DateTime!) {
  field0: test(dt: $var_0)
  __typename
}", query.Query);

            Assert.AreEqual(dateTime, query.Variables["var_0"]);
        }

        [Test]
        public void Query_InputType_CreatesCorrectQuery()
        {
            var networkClient = Substitute.For<DefaultNetworkClient>();

            var client = new TestClient(networkClient);

            var query = client.CreateQuery(e => new
            {
                test = e.InputObj(new SomeInputObject { 
                    Faz = 42,
                    Bar = null
                })
            });

            AssertUtils.AreEqualIgnoreLineBreaks(@"query Query($var_0: SomeInputObject) {
  field0: test(input: $var_0)
  __typename
}", query.Query);

            Assert.AreEqual(42, ((SomeInputObject)query.Variables["var_0"]).Faz);
            Assert.AreEqual(null, ((SomeInputObject)query.Variables["var_0"]).Bar);
        }

        [Test]
        public void Query_ArrayInputType_CreatesCorrectQuery()
        {
            var networkClient = Substitute.For<DefaultNetworkClient>();

            var client = new TestClient(networkClient);

            var query = client.CreateQuery(e => new
            {
                test = e.ArrayOfInputObj(new[]
                {
                    new SomeInputObject { Faz = 42, Bar = null },
                    new SomeInputObject { Faz = 12, Bar = "test" }
                })
            });

            AssertUtils.AreEqualIgnoreLineBreaks(@"query Query($var_0: [SomeInputObject]) {
  field0: test(input: $var_0)
  __typename
}", query.Query);
        }

        class TestQuery
        {
            [GraphQLField("test", "Int!")]
            public int ArrayAsParameter([GraphQLArgument("arr", "[Int!]!")] IEnumerable<int> arr) { throw new InvalidOperationException(); }

            [GraphQLField("test", "Int!")]
            public int NullableParam([GraphQLArgument("arr", "Int")] int? arr) { throw new InvalidOperationException(); }

            [GraphQLField("test", "Int!")]
            public int DateTimeParam([GraphQLArgument("dt", "DateTime!")] DateTime dt) { throw new InvalidOperationException(); }

            [GraphQLField("test", "Int!")]
            public int InputObj([GraphQLArgument("input", "SomeInputObject")] SomeInputObject input) { throw new InvalidOperationException(); }

            [GraphQLField("test", "Int!")]
            public int ArrayOfInputObj([GraphQLArgument("input", "[SomeInputObject]")] IEnumerable<SomeInputObject> input) { throw new InvalidOperationException(); }
        }

        [GraphQLType("SomeInputObject")]
        public class SomeInputObject
        {
            [GraphQLField("foo", "String")]
            public virtual String Foo
            {
                get;
                set;
            }

            [GraphQLField("bar", "String")]
            public virtual String Bar
            {
                get;
                set;
            }

            [GraphQLField("faz", "Int")]
            public virtual Int32? Faz
            {
                get;
                set;
            }
        }

        class TestClient : GraphQLCLient<TestQuery>
        {
            public TestClient(DefaultNetworkClient client) : base(client)
            {
            }
        }
    }
}
