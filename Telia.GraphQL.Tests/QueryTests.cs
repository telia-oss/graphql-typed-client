using Telia.GraphQL.Client.Attributes;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Telia.GraphQL.Tests
{
    [TestFixture]
    public class QueryTests
    {
        [Test]
        public void CreateQuery_RequestForSimpleScalar_GeneratesCorrectQuery()
        {
            var client = new TestClient(null);

            var query = client.CreateQuery(e => new
            {
                test = e.Test,
                test2 = e.Object.TestWithParams(0.5f)
            });

            AssertUtils.AreEqualIgnoreLineBreaks(@"{
  field0: test
  field1: object{
    field0: testWithParams(x: 0.5)
  }
}", query);
        }

        [Test]
        public void CreateQuery_RequestForSimpleObject_GeneratesCorrectQuery()
        {
            var client = new TestClient(null);

            var query = client.CreateQuery(e => new
            {
                o = new
                {
                    b = e.Object.Test
                }
            });

            AssertUtils.AreEqualIgnoreLineBreaks(@"{
  field0: object{
    field0: test
  }
}", query);
        }

        [Test]
        public void CreateQuery_RequestForComplexObject_GeneratesCorrectQuery()
        {
            var client = new TestClient(null);

            var query = client.CreateQuery(e => new
            {
                o = e.Complex.Test,
                b = e.Complex.Complex.Complex.Test,
                c = e.Complex.Complex.Simple.Test
            });

            AssertUtils.AreEqualIgnoreLineBreaks(@"{
  field0: complex{
    field0: test
    field1: complex{
      field0: complex{
        field0: test
      }
      field1: simple{
        field0: test
      }
    }
  }
}", query);
        }

        [Test]
        public void CreateQuery_RequestForComplexObjectWithParams_GeneratesCorrectQuery()
        {
            var client = new TestClient(null);

            var query = client.CreateQuery(e => new
            {
                o = e.ComplexWithParams(null).Test,
                b = e.ComplexWithParams("test4").Complex.ComplexWithParams("test2").Complex.Test,
                c = e.ComplexWithParams("test1").ComplexWithParams("test3").Simple.Test
            });

            AssertUtils.AreEqualIgnoreLineBreaks(@"{
  field0: complexWithParams(name: null){
    field0: test
  }
  field1: complexWithParams(name: ""test4""){
    field0: complex{
      field0: complexWithParams(name: ""test2""){
        field0: complex{
          field0: test
        }
      }
    }
  }
  field2: complexWithParams(name: ""test1""){
    field0: complexWithParams(name: ""test3""){
      field0: simple{
        field0: test
      }
    }
  }
}", query);
        }


        [Test]
        public void CreateQuery_RequestForComplexObjectWithExernalParams_GeneratesCorrectQuery()
        {
            var client = new TestClient(null);

            var test1 = "test1";
            var test2 = "test2";
            var test3 = "test3";
            var test4 = "test4";

            var query = client.CreateQuery(e => new
            {
                o = e.ComplexWithParams(test1).Test,
                b = e.ComplexWithParams(test4).Complex.ComplexWithParams(test2).Complex.Test,
                c = e.ComplexWithParams(test1).ComplexWithParams2(test3, test4).Simple.Test
            });

            AssertUtils.AreEqualIgnoreLineBreaks(@"{
  field0: complexWithParams(name: ""test1""){
    field0: test
    field1: complexWithParams2(name: ""test3"", surname: ""test4""){
      field0: simple{
        field0: test
      }
    }
  }
  field1: complexWithParams(name: ""test4""){
    field0: complex{
      field0: complexWithParams(name: ""test2""){
        field0: complex{
          field0: test
        }
      }
    }
  }
}", query);
        }

        [Test]
        public void CreateQuery_RequestForComplexObjectWithExernalParamsInObject_GeneratesCorrectQuery()
        {
            var client = new TestClient(null);

            var input = new
            {
                test1 = "test1",
                others = new
                {
                    test2 = "test2",
                    others = new
                    {
                        test3 = "test3",
                        others = new
                        {
                            test4 = "test4"
                        }
                    }
                }
            };

            var query = client.CreateQuery(e => new
            {
                o = e.ComplexWithParams(input.test1).Test,
                b = e.ComplexWithParams(input.others.others.others.test4).Complex.ComplexWithParams(input.others.test2).Complex.Test,
                c = e.ComplexWithParams(input.test1).ComplexWithParams(input.others.others.test3).Simple.Test
            });

            AssertUtils.AreEqualIgnoreLineBreaks(@"{
  field0: complexWithParams(name: ""test1""){
    field0: test
    field1: complexWithParams(name: ""test3""){
      field0: simple{
        field0: test
      }
    }
  }
  field1: complexWithParams(name: ""test4""){
    field0: complex{
      field0: complexWithParams(name: ""test2""){
        field0: complex{
          field0: test
        }
      }
    }
  }
}", query);
        }

		[Test]
		public void CreateQuery_RequestWithSelect_GeneratesCorrectQuery()
		{
			var client = new TestClient(null);

			var query = client.CreateQuery(e => new
			{
				o = e.Complex.ComplexArray.Select(x => x.Test)
			});

            AssertUtils.AreEqualIgnoreLineBreaks(@"{
  field0: complex{
    field0: complexArray{
      field0: test
    }
  }
}", query);
		}

		[Test]
		public void CreateQuery_RequestWithSelectAndVariableOutsideScope_GeneratesCorrectQuery()
		{
			var client = new TestClient(null);

			var query = client.CreateQuery(e => new
			{
				o = e.Complex.ComplexArray.Select(x => new { a = x.Test, b = e.Test })
			});

			AssertUtils.AreEqualIgnoreLineBreaks(@"{
  field0: complex{
    field0: complexArray{
      field0: test
    }
  }
  field1: test
}", query);
		}


		[Test]
        public void Query_RequestForSimpleScalar_ReturnsCorrectData()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<string>()).Returns("{ data: { field0: 42 } }");

            var client = new TestClient(networkClient);

            var data = client.Query(e => new
            {
                test = e.Test
            });

            Assert.AreEqual(42, data.Data.test);
        }

        [Test]
        public void Query_RequestForMultipleSimpleScalars_ReturnsCorrectData()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<string>()).Returns("{ data: { field0: 42 } }");

            var client = new TestClient(networkClient);

            var data = client.Query(e => new
            {
                test = e.Test,
                test2 = e.Test,
                test3 = e.Test
            });

            Assert.AreEqual(42, data.Data.test);
            Assert.AreEqual(42, data.Data.test2);
            Assert.AreEqual(42, data.Data.test3);
        }

        [Test]
        public void Query_NestedObject_ReturnsCorrectData()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<string>()).Returns("{ data: { field0: 42 } }");

            var client = new TestClient(networkClient);

            var data = client.Query(e => new
            {
                test = e.Test,
                obj = new
                {
                    e.Test,
                    obj = new
                    {
                        e.Test
                    }
                }
            });

            Assert.AreEqual(42, data.Data.test);
            Assert.AreEqual(42, data.Data.obj.Test);
            Assert.AreEqual(42, data.Data.obj.obj.Test);
        }

        [Test]
        public void Query_ComplexObject_ReturnsCorrectData()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<string>())
                .Returns("{ data: { field0: 42, field1: { field0: 12, field1: { field0: { field0: 10 } } } } }");

            var client = new TestClient(networkClient);

            var data = client.Query(e => new
            {
                test = e.Test,
                a = e.Complex.Test,
                b = e.Complex.Complex.Complex.Test,
                c = e.Complex.Complex.Complex.Test
            });

            Assert.AreEqual(42, data.Data.test);
            Assert.AreEqual(12, data.Data.a);
            Assert.AreEqual(10, data.Data.b);
            Assert.AreEqual(10, data.Data.c);
        }

        [Test]
        public void Query_ComplexObjectNestedResult_ReturnsCorrectData()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<string>())
                .Returns("{ data: { field0: 42, field1: { field0: 12, field1: { field0: { field0: 10 } } } } }");

            var client = new TestClient(networkClient);

            var data = client.Query(e => new
            {
                test = e.Test,
                a = e.Complex.Test,
                obj = new
                {
                    b = e.Complex.Complex.Complex.Test,
                    obj = new
                    {
                        c = e.Complex.Complex.Complex.Test
                    }
                }
            });

            Assert.AreEqual(42, data.Data.test);
            Assert.AreEqual(12, data.Data.a);
            Assert.AreEqual(10, data.Data.obj.b);
            Assert.AreEqual(10, data.Data.obj.obj.c);
        }

        [Test]
        public void Query_ComplexObjectNestedResult_HandlesNull()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<string>()).Returns("{ data: { field0: null, field1: null } }");

            var client = new TestClient(networkClient);

            var data = client.Query(e => new
            {
                test = e.Test,
                a = e.Complex.Test,
                obj = new
                {
                    b = e.Complex.Complex.Complex.Test,
                    obj = new
                    {
                        c = e.Complex.Complex.Complex.Test
                    }
                }
            });

            Assert.AreEqual(0, data.Data.test);
            Assert.AreEqual(0, data.Data.a);
            Assert.AreEqual(0, data.Data.obj.b);
            Assert.AreEqual(0, data.Data.obj.obj.c);
        }

        [Test]
        public void Query_RequestForScalarArray_ReturnsCorrectData()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<string>())
                .Returns("{ data: { field0: { field0: [1, 2, 3] } } }");

            var client = new TestClient(networkClient);

            var data = client.Query(e => new
            {
                test = e.Object.TestArray
            });

            Assert.AreEqual(new int[] { 1, 2, 3 }, data.Data.test);
        }

        [Test]
        public void Query_RequestForObjectArray_ReturnsCorrectData()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<string>())
                .Returns("{ data: { field0: { field0: [{ field0: 1 }, { field0: 2 }] } } }");

            var client = new TestClient(networkClient);

            var data = client.Query(e => new
            {
                test = e.Complex.ComplexArray.Select(x => new
                {
                    member = x.Test
                })
            });

            Assert.AreEqual(new[]
            { new { member = 1 }, new { member = 2 } },
            data.Data.test);
        }

        [Test]
        public void Query_RequestForObjectArrayNested_ReturnsCorrectData()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<string>())
                .Returns("{ data: { field0: { field0: [{ field0: 1, field1: [{ field0: 2 }, { field0: 3 }] }] } } }");

            var client = new TestClient(networkClient);

            var data = client.Query(e => new
            {
                test = e.Complex.ComplexArray.Select(x => new
                {
                    member = x.Test,
                    nested = x.ComplexArray.Select(y => new
                    {
                        member2 = y.Test
                    })
                })
            });

            Assert.AreEqual(1, data.Data.test.First().member);
            Assert.AreEqual(2, data.Data.test.First().nested.First().member2);
            Assert.AreEqual(3, data.Data.test.First().nested.Last().member2);
        }

		[Test]
		public void Query_RequestForObjectArrayNestedOutsideContext_ReturnsCorrectData()
		{
			var networkClient = Substitute.For<INetworkClient>();
			networkClient.Send(Arg.Any<string>()).Returns(@"{ data: {
""field0"": {
  ""field0"": [
    { ""field0"": 1, ""field1"": [
    { ""field0"": 2 }
  ] }
  ]
},
""field1"": 42
} }");

			var client = new TestClient(networkClient);

			var data = client.Query(e => new
			{
				test = e.Complex.ComplexArray.Select(x => new
				{
					member = x.Test,
					member2 = e.Test,
					nested = x.ComplexArray.Select(y => new
					{
						member3 = y.Test,
						member4 = x.Test,
						member5 = e.Test,
					})
				}),
				test2 = e.Test
			});

			Assert.AreEqual(42, data.Data.test2);
			Assert.AreEqual(1, data.Data.test.First().member);
			Assert.AreEqual(42, data.Data.test.First().member2);
			Assert.AreEqual(2, data.Data.test.First().nested.First().member3);
			Assert.AreEqual(1, data.Data.test.First().nested.First().member4);
			Assert.AreEqual(42, data.Data.test.First().nested.First().member5);
		}

		[Test]
        public void Query_RequestWithStringFormatting_ReturnsCorrectData()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<string>()).Returns("{ data: { field0: 42 } }");

            var client = new TestClient(networkClient);

            var data = client.Query(e => new
            {
                test = $"{e.Test}-{e.Test}"
            });

            Assert.AreEqual("42-42", data.Data.test);
        }

        [Test]
        public void Query_RequestWithStringConcattenation_ReturnsCorrectData()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<string>()).Returns("{ data: { field0: 42 } }");

            var client = new TestClient(networkClient);

            var data = client.Query(e => new
            {
                test = e.Test + "-" + e.Test
            });

            Assert.AreEqual("42-42", data.Data.test);
        }

        [Test]
        public void Query_RequestWithBinaryOperationsAndMethodInvocation_ReturnsCorrectData()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<string>()).Returns("{ data: { field0: 42 } }");

            var client = new TestClient(networkClient);

            var data = client.Query(e => new
            {
                test = (e.Test + 12 / 2 * e.Test).ToString()
            });

            Assert.AreEqual("294", data.Data.test);
        }

		[Test]
		public void Query_WithNestedSelectOutsideScope_ReturnsCorrectData()
		{
			var networkClient = Substitute.For<INetworkClient>();
			networkClient.Send(Arg.Any<string>()).Returns(@"{ data: {
""field0"": {
  ""field0"": [
    { ""field0"": 42 }
  ]
}
} }");

			var client = new TestClient(networkClient);

			var data = client.Query(e => new
			{
				test = e.Complex.ComplexArray.Select(x => new
				{
					nested = e.Complex.ComplexArray.Select(y => new
					{
						member3 = y.Test,
					})
				})
			});

			Assert.AreEqual(42, data.Data.test.First().nested.First().member3);
		}

        [Test]
        public void Query_WithDataAndError_ReturnsCorrectData()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<string>())
                .Returns(@"{
data: { field0: 42 },
errors: [
    {
        ""message"": ""something happened"",
        ""locations"": [{ ""line"": 2, ""column"": 4 }],
        ""path"": [ ""foo"", ""bar"", 1, ""faa"" ]
    }
]
}");

            var client = new TestClient(networkClient);

            var data = client.Query(e => new { a = e.Test });

            Assert.AreEqual(42, data.Data.a);
            Assert.AreEqual("something happened", data.Errors.First().Message);
            Assert.AreEqual(2, data.Errors.First().Locations.First().Line);
            Assert.AreEqual(4, data.Errors.First().Locations.First().Column);

            Assert.AreEqual("foo", data.Errors.First().Path.ElementAt(0));
            Assert.AreEqual("bar", data.Errors.First().Path.ElementAt(1));
            Assert.AreEqual(1, data.Errors.First().Path.ElementAt(2));
            Assert.AreEqual("faa", data.Errors.First().Path.ElementAt(3));
        }

        [Test]
        public void Query_WithError_ReturnsCorrectData()
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

            var data = client.Query(e => new { a = e.Test });

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
            [GraphQLField("test")]
            public int Test { get; set; }

            [GraphQLField("object")]
            public SimpleObject Object { get; set; }

            [GraphQLField("complex")]
            public ComplexObject Complex { get; set; }

            [GraphQLField("complexWithParams")]
            public ComplexObject ComplexWithParams(string name) { throw new InvalidOperationException(); }
        }

        private class SimpleObject
        {
            [GraphQLField("test")]
            public int Test { get; set; }

            [GraphQLField("testWithParams")]
            public int TestWithParams(float x) { throw new InvalidOperationException(); }

            [GraphQLField("testArray")]
            public IEnumerable<int> TestArray { get; set; }
        }

        private class ComplexObject
        {
            [GraphQLField("test")]
            public int Test { get; set; }

            [GraphQLField("simple")]
            public SimpleObject Simple { get; set; }

            [GraphQLField("complex")]
            public ComplexObject Complex { get; set; }

            [GraphQLField("complexWithParams")]
            public ComplexObject ComplexWithParams(string name) { throw new InvalidOperationException(); }

            [GraphQLField("complexWithParams2")]
            public ComplexObject ComplexWithParams2(string name, string surname) { throw new InvalidOperationException(); }

            [GraphQLField("complexArray")]
            public IEnumerable<ComplexObject> ComplexArray { get; set; }
        }

        private class TestClient : GraphQLCLient<TestQuery>
        {
            public TestClient(INetworkClient client) : base(client)
            {
            }
        }
    }
}
