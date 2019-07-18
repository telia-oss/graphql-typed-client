using Telia.GraphQL.Client.Attributes;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Telia.GraphQL.Client;
using System.Linq;

namespace Telia.GraphQL.Tests
{
    [TestFixture]
    public class MutationTests
    {
        [Test]
        public void Mutation_RequestForSimpleScalar_ReturnsCorrectData()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<string>()).Returns("{ data: { field0: 42 } }");

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
            var networkClient = Substitute.For<INetworkClient>();
            var client = new TestClient(networkClient);

            var mutation = client.CreateMutation(e => new
            {
                test = e.SomeOtherMutation(new SimpleObject()
                {
                    Test = 1,
                    TestArray = new int[] { 2,3,4 }
                })
            });

            Assert.AreEqual(@"mutation {
  field0: someMutation(input: {test: 1, stringTest: null, testArray: [2, 3, 4], object: null})
}", mutation);
        }

        [Test]
        public void CreateMutation_NestedInputObjectWith_CreatesCorrectQuery()
        {
            var networkClient = Substitute.For<INetworkClient>();
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

            Assert.AreEqual(@"mutation {
  field0: someMutation(input: {test: 0, stringTest: null, testArray: null, object: {test: 0, stringTest: null, testArray: null, object: {test: 42, stringTest: null, testArray: null, object: null}}})
}", mutation);
        }

        [Test]
        public void Mutation_RequestForComplicatedObject_ReturnsCorrectData()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<string>()).Returns("{ data: { field0: { field0: \"123\" } }}");

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
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<string>())
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
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<string>())
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

        private class TestQuery
        {

        }

        private class TestMutation
        {
            [GraphQLField("someMutation")]
            public int SomeMutation()
            {
                throw new InvalidOperationException();
            }

            [GraphQLField("someMutation")]
            public int SomeOtherMutation(SimpleObject input)
            {
                throw new InvalidOperationException();
            }

            [GraphQLField("someMutation")]
            public SimpleObject ObjectMutation(SimpleObject input)
            {
                throw new InvalidOperationException();
            }
        }

        private class SimpleObject
        {
            [GraphQLField("test")]
            public int Test { get; set; }

            [GraphQLField("stringTest")]
            public string StringTest { get; set; }

            [GraphQLField("testArray")]
            public IEnumerable<int> TestArray { get; set; }

            [GraphQLField("object")]
            public SimpleObject Object { get; set; }
        }

        private class TestClient : GraphQLCLient<TestQuery, TestMutation>
        {
            public TestClient(INetworkClient client) : base(client)
            {
            }
        }
    }
}
