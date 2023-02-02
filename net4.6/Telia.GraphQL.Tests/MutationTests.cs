using System;
using System.Collections.Generic;
using System.Linq;

using NSubstitute;

using NUnit.Framework;

using Telia.GraphQL.Client;
using Telia.GraphQL.Client.Attributes;

namespace Telia.GraphQL.Tests
{
    [TestFixture]
    public class MutationTests
    {
        [Test]
        public void Mutation_RequestForSimpleScalar_ReturnsCorrectData()
        {
            var networkClient = Substitute.For<DefaultNetworkClient>();
            networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns("{ data: { field0: 42 } }");

            var client = new TestClient(networkClient);

            var data = client.Mutation(e => new
            {
                test = e.SomeMutation()
            });

            Assert.AreEqual(42, data.Data.test);
        }

        [Test]
        public void CreateMutation_InputObjectWithSimpleScalars_CreatesCorrectQuery()
        {
            var networkClient = Substitute.For<DefaultNetworkClient>();
            var client = new TestClient(networkClient);

            var mutation = client.CreateMutation(e => new
            {
                test = e.SomeOtherMutation(new SimpleObject()
                {
                    Test = 1,
                    TestArray = new int[] { 2,3,4 }
                })
            });

            AssertUtils.AreEqualIgnoreLineBreaks(@"mutation Mutation($var_0: SimpleObject) {
  field0: someMutation(input: $var_0)
  __typename
}", mutation.Query);

            Assert.AreEqual(1, ((SimpleObject)mutation.Variables["var_0"]).Test);
            Assert.AreEqual(new[] {2, 3, 4}, ((SimpleObject)mutation.Variables["var_0"]).TestArray);
        }

        [Test]
        public void CreateMutation_NestedInputObjectWith_CreatesCorrectQuery()
        {
            var networkClient = Substitute.For<DefaultNetworkClient>();
            var client = new TestClient(networkClient);

            var mutation = client.CreateMutation(e => new
            {
                test = e.SomeOtherMutation(new SimpleObject()
                {
                    Object = new SimpleObject
                    {
                        Object = new SimpleObject
                        {
                            Test = 42
                        }
                    }
                })
            });

            AssertUtils.AreEqualIgnoreLineBreaks(@"mutation Mutation($var_0: SimpleObject) {
  field0: someMutation(input: $var_0)
  __typename
}", mutation.Query);

            Assert.AreEqual(42, ((SimpleObject)mutation.Variables["var_0"]).Object.Object.Test);
        }

        [Test]
        public void Mutation_RequestForComplicatedObject_ReturnsCorrectData()
        {
            var networkClient = Substitute.For<DefaultNetworkClient>();
            networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns("{ data: { field0: { field0: \"123\" } }}");

            var client = new TestClient(networkClient);

            var data = client.Mutation(e => new
            {
                test = e.ObjectMutation(new SimpleObject { Test = 123 }).StringTest
            });

            Assert.AreEqual("123", data.Data.test);
        }

        [Test]
        public void Mutation_WithDataAndError_ReturnsCorrectData()
        {
            var networkClient = Substitute.For<DefaultNetworkClient>();
            networkClient.Send(Arg.Any<GraphQLQueryInfo>())
                .Returns(@"{
data: { field0: { field0: ""123"" } },
errors: [
    {
        ""message"": ""something happened"",
        ""locations"": [{ ""line"": 2, ""column"": 4 }],
        ""path"": [ ""foo"", ""bar"", 1, ""faa"" ]
    }
]
}");

            var client = new TestClient(networkClient);

            var data = client.Mutation(e => new { a = e.ObjectMutation(new SimpleObject { Test = 123 }).StringTest });

            Assert.AreEqual("123", data.Data.a);
            Assert.AreEqual("something happened", data.Errors.First().Message);
            Assert.AreEqual(2, data.Errors.First().Locations.First().Line);
            Assert.AreEqual(4, data.Errors.First().Locations.First().Column);

            Assert.AreEqual("foo", data.Errors.First().Path.ElementAt(0));
            Assert.AreEqual("bar", data.Errors.First().Path.ElementAt(1));
            Assert.AreEqual(1, data.Errors.First().Path.ElementAt(2));
            Assert.AreEqual("faa", data.Errors.First().Path.ElementAt(3));
        }

        [Test]
        public void Mutation_WithError_ReturnsCorrectData()
        {
            var networkClient = Substitute.For<DefaultNetworkClient>();
            networkClient.Send(Arg.Any<GraphQLQueryInfo>())
                .Returns(@"{
data: null,
errors: [
    {
        ""message"": ""something happened"",
        ""locations"": [{ ""line"": 2, ""column"": 4 }],
        ""path"": [ ""foo"", ""bar"", 1, ""faa"" ]
    }
]
}");

            var client = new TestClient(networkClient);

            var data = client.Mutation(e => new { a = e.ObjectMutation(new SimpleObject { Test = 123 }).StringTest });

            Assert.AreEqual(null, data.Data);

            Assert.AreEqual("something happened", data.Errors.First().Message);
            Assert.AreEqual(2, data.Errors.First().Locations.First().Line);
            Assert.AreEqual(4, data.Errors.First().Locations.First().Column);

            Assert.AreEqual("foo", data.Errors.First().Path.ElementAt(0));
            Assert.AreEqual("bar", data.Errors.First().Path.ElementAt(1));
            Assert.AreEqual(1, data.Errors.First().Path.ElementAt(2));
            Assert.AreEqual("faa", data.Errors.First().Path.ElementAt(3));
        }

        class TestQuery
        {

        }

        class TestMutation
        {
            [GraphQLField("someMutation", "Int!")]
            public int SomeMutation()
            {
                throw new InvalidOperationException();
            }

            [GraphQLField("someMutation", "Int!")]
            public int SomeOtherMutation([GraphQLArgument("input", "SimpleObject")] SimpleObject input)
            {
                throw new InvalidOperationException();
            }

            [GraphQLField("someMutation", "SimpleObject")]
            public SimpleObject ObjectMutation([GraphQLArgument("input", "SimpleObject")] SimpleObject input)
            {
                throw new InvalidOperationException();
            }
        }

        class SimpleObject
        {
            [GraphQLField("test", "Int!")]
            public int Test { get; set; }

            [GraphQLField("stringTest", "String")]
            public string StringTest { get; set; }

            [GraphQLField("testArray", "[Int!]")]
            public IEnumerable<int> TestArray { get; set; }

            [GraphQLField("object", "SimpleObject")]
            public SimpleObject Object { get; set; }
        }

        class TestClient : GraphQLCLient<TestQuery, TestMutation>
        {
            public TestClient(DefaultNetworkClient client) : base(client)
            {
            }
        }
    }
}
