using Telia.GraphQL.Client.Attributes;
using NUnit.Framework;
using System.Collections.Generic;
using NSubstitute;
using System.Linq;

namespace Telia.GraphQL.Tests
{
	[TestFixture]
    public class TypeCheckTests
	{
        [Test]
        public void Query_RequiredIntResultHasTrue_ConvertsValue()
        {
			var networkClient = Substitute.For<INetworkClient>();
			networkClient.Send(Arg.Any<string>()).Returns("{ field0: true }");

			var client = new TestClient(networkClient);

            var result = client.Query(e => new
            {
                test = e.Test
            });

            Assert.AreEqual(1, result.test);
        }

		[Test]
		public void Query_RequiredIntResultHasFalse_ConvertsValue()
		{
			var networkClient = Substitute.For<INetworkClient>();
			networkClient.Send(Arg.Any<string>()).Returns("{ field0: false }");

			var client = new TestClient(networkClient);

			var result = client.Query(e => new
			{
				test = e.Test
			});

			Assert.AreEqual(0, result.test);
		}

		[Test]
		public void Query_RequiredIntResultHasArray_ReturnsDefaultValue()
		{
			var networkClient = Substitute.For<INetworkClient>();
			networkClient.Send(Arg.Any<string>()).Returns("{ field0: [] }");

			var client = new TestClient(networkClient);

			var result = client.Query(e => new
			{
				test = e.Test
			});

			Assert.AreEqual(0, result.test);
		}

		[Test]
		public void Query_RequiredIntResultHasObject_ReturnsDefaultValue()
		{
			var networkClient = Substitute.For<INetworkClient>();
			networkClient.Send(Arg.Any<string>()).Returns("{ field0: {} }");

			var client = new TestClient(networkClient);

			var result = client.Query(e => new
			{
				test = e.Test
			});

			Assert.AreEqual(0, result.test);
		}

		[Test]
		public void Query_RequiredIntResultHasNull_ReturnsDefaultValue()
		{
			var networkClient = Substitute.For<INetworkClient>();
			networkClient.Send(Arg.Any<string>()).Returns("{ field0: null }");

			var client = new TestClient(networkClient);

			var result = client.Query(e => new
			{
				test = e.Test
			});

			Assert.AreEqual(0, result.test);
		}

		[Test]
		public void Query_RequiredArrayResultHasInt_ReturnsNull()
		{
			var networkClient = Substitute.For<INetworkClient>();
			networkClient.Send(Arg.Any<string>()).Returns("{ field0: 1 }");

			var client = new TestClient(networkClient);

			var result = client.Query(e => new
			{
				test = e.TestArray
			});

			Assert.AreEqual(null, result.test);
		}

		[Test]
		public void Query_RequiredArrayOfIntsResultHasArrayOfObjects_ReturnsArrayOfDefaults()
		{
			var networkClient = Substitute.For<INetworkClient>();
			networkClient.Send(Arg.Any<string>()).Returns("{ field0: [{}, {}, {}] }");

			var client = new TestClient(networkClient);

			var result = client.Query(e => new
			{
				test = e.TestArray
			});

			Assert.AreEqual(0, result.test.ElementAt(0));
			Assert.AreEqual(0, result.test.ElementAt(1));
			Assert.AreEqual(0, result.test.ElementAt(2));
		}

		[Test]
		public void Query_RequiredObjectResultHasInt_ReturnsNull()
		{
			var networkClient = Substitute.For<INetworkClient>();
			networkClient.Send(Arg.Any<string>()).Returns("{ field0: 1 }");

			var client = new TestClient(networkClient);

			var result = client.Query(e => new
			{
				test = e.TestObject
			});

			Assert.AreEqual(null, result.test);
		}

		[Test]
		public void Query_RequiredEnumResultHasString_ConvertsValue()
		{
			var networkClient = Substitute.For<INetworkClient>();
			networkClient.Send(Arg.Any<string>()).Returns("{ field0: \"ENUM_2\" }");

			var client = new TestClient(networkClient);

			var result = client.Query(e => new
			{
				test = e.Enum
			});

			Assert.AreEqual(TestEnum.ENUM_2, result.test);
		}

		private enum TestEnum
		{
			ENUM_1,
			ENUM_2
		}

		private class TestQuery
        {
            [GraphQLField("test")]
            public int Test { get; set; }

			[GraphQLField("test2")]
			public IEnumerable<int> TestArray { get; set; }

			[GraphQLField("test3")]
			public TestQuery TestObject { get; set; }

			[GraphQLField("enum")]
			public TestEnum Enum { get; set; }
		}

        private class TestClient : GraphQLCLient<TestQuery>
        {
            public TestClient(INetworkClient client) : base(client)
            {
            }
        }
    }
}
