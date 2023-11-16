using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using Telia.GraphQL.Client;
using Telia.GraphQL.Client.Attributes;

namespace Telia.GraphQL.Tests
{
    [TestFixture]
    public class SerializationTests
    {

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

            var json = JsonConvert.SerializeObject(query, new JsonSerializerSettings()
            {
                Converters = new List<JsonConverter>()
                {
                    new GraphQLObjectConverter()
                },
                Formatting = Formatting.Indented
            });

            AssertUtils.AreEqualIgnoreLineBreaks(
                @"{
  ""query"": ""query Query($var_0: [SomeInputObject]) {\r\n  field0: test(input: $var_0)\r\n  __typename\r\n}"",
  ""variables"": {
    ""var_0"": [
      {
        ""faz"": 42
      },
      {
        ""bar"": ""test"",
        ""faz"": 12
      }
    ]
  }
}",
                json);
        }

        [Test]
        public void Query_ArrayInputTypeWithNull_CreatesCorrectQuery()
        {
            var networkClient = Substitute.For<INetworkClient>();

            var client = new TestClient(networkClient);

            var query = client.CreateQuery(e => new
            {
                test = e.ArrayOfInputObj(new[]
                {
                    new SomeInputObject { Faz = 42, Bar = null },
                   null
                })
            });

            var json = JsonConvert.SerializeObject(query, new JsonSerializerSettings()
            {
                Converters = new List<JsonConverter>()
                {
                    new GraphQLObjectConverter()
                },
                Formatting = Formatting.Indented
            });

            AssertUtils.AreEqualIgnoreLineBreaks(
                @"{
  ""query"": ""query Query($var_0: [SomeInputObject]) {\r\n  field0: test(input: $var_0)\r\n  __typename\r\n}"",
  ""variables"": {
    ""var_0"": [
      {
        ""faz"": 42
      },
      null
    ]
  }
}",
                json);
        }

        [Test]
        public void Query_InputTypeWithNull_CreatesCorrectQuery()
        {
            var networkClient = Substitute.For<INetworkClient>();

            var client = new TestClient(networkClient);

            var query = client.CreateQuery(e => new
            {
                test = e.InputObj(new SomeInputObject { Faz = 42, Bar = null })
            });

            var json = JsonConvert.SerializeObject(query, new JsonSerializerSettings()
            {
                Converters = new List<JsonConverter>()
                {
                    new GraphQLObjectConverter()
                },
                Formatting = Formatting.Indented
            });

            AssertUtils.AreEqualIgnoreLineBreaks(
                @"{
  ""query"": ""query Query($var_0: SomeInputObject) {\r\n  field0: test(input: $var_0)\r\n  __typename\r\n}"",
  ""variables"": {
    ""var_0"": {
      ""faz"": 42
    }
  }
}",
                json);
        }


        private class TestQuery
        {
            [GraphQLField("test", "Int!")]
            public int InputObj([GraphQLArgument("input", "SomeInputObject")] SomeInputObject input) { throw new InvalidOperationException(); }

            [GraphQLField("test", "Int!")]
            public int ArrayOfInputObj([GraphQLArgument("input", "[SomeInputObject]")] IEnumerable<SomeInputObject> input) { throw new InvalidOperationException(); }
        }

        [GraphQLType("SomeInputObject")]
        public class SomeInputObject
        {
            [GraphQLField("bar", "String")] public virtual String Bar { get; set; }

            [GraphQLField("faz", "Int")] public virtual Int32? Faz { get; set; }
        }

        private class TestClient : GraphQLCLient<TestQuery>
        {
            public TestClient(INetworkClient client) : base(client)
            {
            }
        }
    }
}
