﻿using Telia.GraphQL.Client.Attributes;
using NUnit.Framework;
using System.Collections.Generic;
using NSubstitute;
using System.Linq;
using System;
using Telia.GraphQL.Client;

namespace Telia.GraphQL.Tests
{
    [TestFixture]
    public class TypeCheckTests
	{
        [Test]
        public void Query_RequiredIntResultHasTrue_ConvertsValue()
        {
			var networkClient = Substitute.For<INetworkClient>();
			networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns("{ data: { field0: true } }");

			var client = new TestClient(networkClient);

            var result = client.Query(e => new
            {
                test = e.Test
            });

            Assert.AreEqual(1, result.Data.test);
        }

		[Test]
		public void Query_RequiredIntResultHasFalse_ConvertsValue()
		{
			var networkClient = Substitute.For<INetworkClient>();
			networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns("{ data: { field0: false } }");

			var client = new TestClient(networkClient);

			var result = client.Query(e => new
			{
				test = e.Test
			});

			Assert.AreEqual(0, result.Data.test);
		}

		[Test]
		public void Query_RequiredIntResultHasArray_ReturnsDefaultValue()
		{
			var networkClient = Substitute.For<INetworkClient>();
			networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns("{ data: { field0: [] } }");

			var client = new TestClient(networkClient);

			var result = client.Query(e => new
			{
				test = e.Test
			});

			Assert.AreEqual(0, result.Data.test);
		}

		[Test]
		public void Query_RequiredIntResultHasObject_ReturnsDefaultValue()
		{
			var networkClient = Substitute.For<INetworkClient>();
			networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns("{ data: { field0: {} } }");

			var client = new TestClient(networkClient);

			var result = client.Query(e => new
			{
				test = e.Test
			});

			Assert.AreEqual(0, result.Data.test);
		}

		[Test]
		public void Query_RequiredIntResultHasNull_ReturnsDefaultValue()
		{
			var networkClient = Substitute.For<INetworkClient>();
			networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns("{ data: { field0: null } }");

			var client = new TestClient(networkClient);

			var result = client.Query(e => new
			{
				test = e.Test
			});

			Assert.AreEqual(0, result.Data.test);
		}

		[Test]
		public void Query_RequiredArrayResultHasInt_ReturnsNull()
		{
			var networkClient = Substitute.For<INetworkClient>();
			networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns("{ data: { field0: 1 } }");

			var client = new TestClient(networkClient);

			var result = client.Query(e => new
			{
				test = e.TestArray
			});

			Assert.AreEqual(null, result.Data.test);
		}

		[Test]
		public void Query_RequiredArrayOfIntsResultHasArrayOfObjects_ReturnsArrayOfDefaults()
		{
			var networkClient = Substitute.For<INetworkClient>();
			networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns("{ data: { field0: [{}, {}, {}] } }");

			var client = new TestClient(networkClient);

			var result = client.Query(e => new
			{
				test = e.TestArray
			});

			Assert.AreEqual(0, result.Data.test.ElementAt(0));
			Assert.AreEqual(0, result.Data.test.ElementAt(1));
			Assert.AreEqual(0, result.Data.test.ElementAt(2));
		}

		[Test]
		public void Query_RequiredObjectResultHasInt_ReturnsNull()
		{
			var networkClient = Substitute.For<INetworkClient>();
			networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns("{ data: { field0: 1 } }");

			var client = new TestClient(networkClient);

			var result = client.Query(e => new
			{
				test = e.TestObject
			});

			Assert.AreEqual(null, result.Data.test);
		}

		[Test]
		public void Query_RequiredEnumResultHasString_ConvertsValue()
		{
			var networkClient = Substitute.For<INetworkClient>();
			networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns("{ data: { field0: \"ENUM_2\" } }");

			var client = new TestClient(networkClient);

			var result = client.Query(e => new
			{
				test = e.Enum
			});

			Assert.AreEqual(TestEnum.ENUM_2, result.Data.test);
		}

		[Test]
		public void Query_NestedEnum_ConvertsValue()
		{
			var networkClient = Substitute.For<INetworkClient>();
			networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns("{ data: { field0:  { field0: \"ENUM_2\" } } }");

			var client = new TestClient(networkClient);

			var result = client.Query(e => new
			{
				test = e.TestObject.Enum
			});

			Assert.AreEqual(TestEnum.ENUM_2, result.Data.test);
		}

        [Test]
        public void Query_TimeSpan_ConvertsValue()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns("{ data: { field0: \"12:30Z\" } }");

            var client = new TestClient(networkClient);

            var result = client.Query(e => new
            {
                test = e.Time
            });

            Assert.AreEqual(TimeSpan.Parse("12:30"), result.Data.test);
        }

        [Test]
        public void Query_TimeSpan2_ConvertsValue()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns("{ data: { field0: \"12:30:24-07:00\" } }");

            var client = new TestClient(networkClient);

            var result = client.Query(e => new
            {
                test = e.Time
            });

            Assert.AreEqual(TimeSpan.Parse("19:30:24"), result.Data.test);
        }

        [Test]
        public void Query_TimeSpan3_ConvertsValue()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns("{ data: { field0: \"12:30:24.500+05:30\" } }");

            var client = new TestClient(networkClient);

            var result = client.Query(e => new
            {
                test = e.Time
            });

            Assert.AreEqual(TimeSpan.Parse("07:00:24.500"), result.Data.test);
        }

        [Test]
        public void Query_FloatWithBigValue_ConvertsValue()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns("{ data: { field0: { floatTest: 42949673666 } } }");

            var client = new TestClient(networkClient);

            var result = client.Query(e => new
            {
                test = e.Obj
            });

            Assert.AreEqual(42949673666, result.Data.test.FloatTestField);
        }

		private enum TestEnum
		{
			ENUM_1,
			ENUM_2
		}

        [GraphQLType("TestQuery")]
		private class TestQuery
        {
            [GraphQLField("test", "Int!")]
            public int Test { get; set; }

			[GraphQLField("test2", "[Int!]")]
			public IEnumerable<int> TestArray { get; set; }

			[GraphQLField("test3", "TestQuery!")]
			public TestQuery TestObject { get; set; }

			[GraphQLField("enum", "TestEnum!")]
			public TestEnum Enum { get; set; }

            [GraphQLField("time", "TimeSpan!")]
            public TimeSpan Time { get; set; }

            [GraphQLField("obj", "TestObjectWithFloat!")]
            public TestObjectWithFloat Obj { get; set; }
		}

		[GraphQLType("TestObjectWithFloat")]
        private class TestObjectWithFloat
        {
            [GraphQLField("floatTest", "Float")]
            public Single? FloatTestField { get; set; }
		}

        private class TestClient : GraphQLCLient<TestQuery>
        {
            public TestClient(INetworkClient client) : base(client)
            {
            }
        }
    }
}
