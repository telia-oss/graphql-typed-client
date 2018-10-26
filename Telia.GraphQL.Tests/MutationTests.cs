using Telia.GraphQL.Client.Attributes;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Telia.GraphQL.Tests
{
    [TestFixture]
    public class MutationTests
    {
        [Test]
        public void Mutation_RequestForSimpleScalar_ReturnsCorrectData()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<string>()).Returns("{ field0: 42 }");

            var client = new TestClient(networkClient);

            var data = client.Mutation(e => new
            {
                test = e.SomeMutation()
            });

            Assert.AreEqual(42, data.test);
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
            networkClient.Send(Arg.Any<string>()).Returns("{ field0: { field0: \"123\" } }");

            var client = new TestClient(networkClient);

            var data = client.Mutation(e => new
            {
                test = e.ObjectMutation(new SimpleObject { Test = 123 }).StringTest
            });

            Assert.AreEqual("123", data.test);
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
