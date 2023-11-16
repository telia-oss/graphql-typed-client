using Telia.GraphQL.Client.Attributes;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Telia.GraphQL.Client;

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

            AssertUtils.AreEqualIgnoreLineBreaks(@"query Query($var_0: Float!) {
  field0: test
  field1: object{
    field0: testWithParams(x: $var_0)
    __typename
  }
  __typename
}", query.Query);

            Assert.AreEqual(0.5f, query.Variables["var_0"]);
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

            AssertUtils.AreEqualIgnoreLineBreaks(@"query Query {
  field0: object{
    field0: test
    __typename
  }
  __typename
}", query.Query);
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

            AssertUtils.AreEqualIgnoreLineBreaks(@"query Query {
  field0: complex{
    field0: test
    field1: complex{
      field0: complex{
        field0: test
        __typename
      }
      field1: simple{
        field0: test
        __typename
      }
      __typename
    }
    __typename
  }
  __typename
}", query.Query);
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

            AssertUtils.AreEqualIgnoreLineBreaks(@"query Query($var_0: String!, $var_1: String!, $var_2: String!, $var_3: String!) {
  field0: complexWithParams(name: null){
    field0: test
    __typename
  }
  field1: complexWithParams(name: $var_0){
    field0: complex{
      field0: complexWithParams(name: $var_1){
        field0: complex{
          field0: test
          __typename
        }
        __typename
      }
      __typename
    }
    __typename
  }
  field2: complexWithParams(name: $var_2){
    field0: complexWithParams(name: $var_3){
      field0: simple{
        field0: test
        __typename
      }
      __typename
    }
    __typename
  }
  __typename
}", query.Query);

            Assert.AreEqual("test4", query.Variables["var_0"]);
            Assert.AreEqual("test2", query.Variables["var_1"]);
            Assert.AreEqual("test1", query.Variables["var_2"]);
            Assert.AreEqual("test3", query.Variables["var_3"]);
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

            AssertUtils.AreEqualIgnoreLineBreaks(@"query Query($var_0: String!, $var_1: String!, $var_2: String!, $var_3: String!, $var_4: String!) {
  field0: complexWithParams(name: $var_0){
    field0: test
    field1: complexWithParams2(name: $var_1, surname: $var_2){
      field0: simple{
        field0: test
        __typename
      }
      __typename
    }
    __typename
  }
  field1: complexWithParams(name: $var_3){
    field0: complex{
      field0: complexWithParams(name: $var_4){
        field0: complex{
          field0: test
          __typename
        }
        __typename
      }
      __typename
    }
    __typename
  }
  __typename
}", query.Query);

            Assert.AreEqual("test1", query.Variables["var_0"]);
            Assert.AreEqual("test3", query.Variables["var_1"]);
            Assert.AreEqual("test4", query.Variables["var_2"]);
            Assert.AreEqual("test4", query.Variables["var_3"]);
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

            AssertUtils.AreEqualIgnoreLineBreaks(@"query Query($var_0: String!, $var_1: String!, $var_2: String!, $var_3: String!) {
  field0: complexWithParams(name: $var_0){
    field0: test
    field1: complexWithParams(name: $var_1){
      field0: simple{
        field0: test
        __typename
      }
      __typename
    }
    __typename
  }
  field1: complexWithParams(name: $var_2){
    field0: complex{
      field0: complexWithParams(name: $var_3){
        field0: complex{
          field0: test
          __typename
        }
        __typename
      }
      __typename
    }
    __typename
  }
  __typename
}", query.Query);

            Assert.AreEqual("test1", query.Variables["var_0"]);
            Assert.AreEqual("test3", query.Variables["var_1"]);
            Assert.AreEqual("test4", query.Variables["var_2"]);
            Assert.AreEqual("test2", query.Variables["var_3"]);
        }

		[Test]
		public void CreateQuery_RequestWithSelect_GeneratesCorrectQuery()
		{
			var client = new TestClient(null);

			var query = client.CreateQuery(e => new
			{
				o = e.Complex.ComplexArray.Select(x => x.Test)
			});

            AssertUtils.AreEqualIgnoreLineBreaks(@"query Query {
  field0: complex{
    field0: complexArray{
      field0: test
      __typename
    }
    __typename
  }
  __typename
}", query.Query);
		}

		[Test]
		public void CreateQuery_RequestWithSelectAndVariableOutsideScope_GeneratesCorrectQuery()
		{
			var client = new TestClient(null);

			var query = client.CreateQuery(e => new
			{
				o = e.Complex.ComplexArray.Select(x => new { a = x.Test, b = e.Test })
			});

			AssertUtils.AreEqualIgnoreLineBreaks(@"query Query {
  field0: complex{
    field0: complexArray{
      field0: test
      __typename
    }
    __typename
  }
  field1: test
  __typename
}", query.Query);
		}


		[Test]
        public void Query_RequestForSimpleScalar_ReturnsCorrectData()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns("{ data: { field0: 42 } }");

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
            networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns("{ data: { field0: 42 } }");

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
            networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns("{ data: { field0: 42 } }");

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
            networkClient.Send(Arg.Any<GraphQLQueryInfo>())
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
            networkClient.Send(Arg.Any<GraphQLQueryInfo>())
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
            networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns("{ data: { field0: null, field1: null } }");

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
            networkClient.Send(Arg.Any<GraphQLQueryInfo>())
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
            networkClient.Send(Arg.Any<GraphQLQueryInfo>())
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
            networkClient.Send(Arg.Any<GraphQLQueryInfo>())
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
			networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns(@"{ data: {
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
            networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns("{ data: { field0: 42 } }");

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
            networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns("{ data: { field0: 42 } }");

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
            networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns("{ data: { field0: 42 } }");

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
			networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns(@"{ data: {
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
            networkClient.Send(Arg.Any<GraphQLQueryInfo>())
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

        [Test]
        public void Query_WithEnum_ReturnsCorrectData()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns(@"{ data: {
""field0"": ""TEST2""
} }");

            var client = new TestClient(networkClient);

            var data = client.Query(e => new
            {
                test = e.TestEnum,
            });

            Assert.AreEqual(TestEnum.TEST2, data.Data.test);
        }

        [Test]
        public void Query_WithNestedEnum_ReturnsCorrectData()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns(@"{ data: {
""field0"": { ""field0"": ""TEST2"" }
} }");

            var client = new TestClient(networkClient);

            var data = client.Query(e => new
            {
                test = e.Object.TestEnum,
            });

            Assert.AreEqual(TestEnum.TEST2, data.Data.test);
        }

        [Test]
        public void Query_WithDate_ReturnsCorrectData()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns(@"{ data: {
""field0"": ""2019-09-01T22:00:08.000Z""
} }");

            var client = new TestClient(networkClient);

            var data = client.Query(e => new
            {
                test = e.Date,
            });

            Assert.AreEqual(DateTime.Parse("2019-09-01T22:00:08.000Z").ToUniversalTime(), data.Data.test);
        }

        [Test]
        public void Query_WithOnlyDate_ReturnsCorrectData()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns(@"{ data: {
""field0"": ""2019-09-01""
} }");

            var client = new TestClient(networkClient);

            var data = client.Query(e => new
            {
                test = e.Date,
            });

            Assert.AreEqual(DateTime.Parse("2019-09-01").ToUniversalTime(), data.Data.test);
        }

        [Test]
        public void Query_WithDateAsNull_ReturnsCorrectData()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns(@"{ data: {
""field0"": null
} }");

            var client = new TestClient(networkClient);

            var data = client.Query(e => new
            {
                test = e.Date,
            });

            Assert.AreEqual(null, data.Data.test);
        }

        [Test]
        public void Query_WithTernaryOperator_ReturnsCorrectData()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns(@"{ data: {
""field0"": ""2019-09-01T22:00:08.000Z""
} }");

            var client = new TestClient(networkClient);

            var data = client.Query(e => new
            {
                test = e.Date.HasValue ? true : false,
            });

            Assert.AreEqual(true, data.Data.test);
        }

        [Test]
        public void Query_WithMoreComplicatedTernaryOperator_ReturnsCorrectData()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns(@"{ data: {
""field0"": { ""field0"": { ""field0"": { ""field0"": { ""field0"": 1 }, ""field1"": { ""field0"": { ""field0"": ""2019-09-01T22:00:08.000Z"" } } } } } } }");

            var client = new TestClient(networkClient);

            var data = client.Query(e => new
            {
                test = e.Complex.Complex.ComplexWithParams("test").Simple.Test == 1
                    ? e.Complex.Complex.ComplexWithParams("test").Complex.Simple.Date.Value
                    : DateTime.MinValue,
            });

            Assert.AreEqual(DateTime.Parse("2019-09-01T22:00:08.000Z").ToUniversalTime(), data.Data.test);
        }

        [Test]
        public void Query_WithDateTime_IsPossibleToParse()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns(@"{ data: { ""field0"": ""2020-05-20T00:00:00.000"" } }");

            var client = new TestClient(networkClient);

            var data = client.Query(e => new
            {
                test = e.Date
            });

            Assert.AreEqual(DateTime.Parse("2020-05-20T00:00:00.000"), data.Data.test);
        }

        [Test]
        public void Query_WithNestedSelects_ReturnsCorrectData()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns(@"{ ""data"": { ""field0"": { ""field0"": [{ ""field0"": [ { ""field0"": 42 } ] }] } } }");

            var client = new TestClient(networkClient);

            var data = client.Query(e => new
            {
                test = e.Complex.ComplexArray.Select(x => x.ComplexArray.Select(y => y.Test))
            });

            Assert.AreEqual(42, data.Data.test.Single().Single());
        }

        [Test]
        public void Query_WithNestedSelectsWithParams_ReturnsCorrectData()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns(@"{ ""data"": { ""field0"": { ""field0"": [{ ""field0"": [ { ""field0"": 42 } ] }] } } }");

            var client = new TestClient(networkClient);

            var data = client.Query(e => new
            {
                test = e.Complex.ComplexArray.Select(x => x.ComplexArrayWithParams("test").Select(y => y.Test))
            });

            Assert.AreEqual(42, data.Data.test.Single().Single());
        }

        [Test]
        public void Query_WithFragment_CreatesCorrectQuery()
        {
            var networkClient = Substitute.For<INetworkClient>();
            var client = new TestClient(networkClient);

            var query = client.CreateQuery(e => new
            {
                test = (e.SimpleInterface as SimpleObject).TestEnum
            });

            AssertUtils.AreEqualIgnoreLineBreaks(@"query Query {
  field0: simpleInterface{
    ... on SimpleObject {
      field0: testEnum
      __typename
    }
    __typename
  }
  __typename
}", query.Query);
        }

        [Test]
        public void Query_WithFragmentInsideSelect_CreatesCorrectQuery()
        {
            var networkClient = Substitute.For<INetworkClient>();
            var client = new TestClient(networkClient);

            var query = client.CreateQuery(e => new
            {
                test = e.Complex.ComplexArray.Select(a => (a.SimpleInterface as SimpleObject).TestEnum)
            });

            AssertUtils.AreEqualIgnoreLineBreaks(@"query Query {
  field0: complex{
    field0: complexArray{
      field0: simpleInterface{
        ... on SimpleObject {
          field0: testEnum
          __typename
        }
        __typename
      }
      __typename
    }
    __typename
  }
  __typename
}", query.Query);
        }

        [Test]
        public void Query_WithFragmentInsideSelectAndMultipleProperties_CreatesCorrectQueryThatDoesntCollideWithPropertyNames()
        {
            var networkClient = Substitute.For<INetworkClient>();
            var client = new TestClient(networkClient);

            var query = client.CreateQuery(e => new
            {
                test = e.Complex.ComplexArray.Select(a => new {
                    test1 = a.SimpleInterface.Test,
                    test2 = (a.SimpleInterface as SimpleObject).TestEnum
                })
            });

            AssertUtils.AreEqualIgnoreLineBreaks(@"query Query {
  field0: complex{
    field0: complexArray{
      field0: simpleInterface{
        field0: test
        ... on SimpleObject {
          field1: testEnum
          __typename
        }
        __typename
      }
      __typename
    }
    __typename
  }
  __typename
}", query.Query);
        }

        [Test]
        public void Query_WithFragmentInsideSelectAndMultipleProperties_ReturnsCorrectData()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns(@"{ ""data"": {
  ""field0"": {
    ""field0"": [{
      ""field0"": {
        ""field0"": 44,
        ""field1"": ""TEST2"",
        ""field2"": 44,
        ""field3"": [1, 2, 3],
        ""__typename"": ""SimpleObject""
      }
    }]
  }
} }");
            var client = new TestClient(networkClient);

            var response = client.Query(e => new
            {
                test = e.Complex.ComplexArray.Select(a => new {
                    test1 = a.SimpleInterface.Test,
                    test2 = (a.SimpleInterface as SimpleObject).TestEnum,
                    test3 = (a.SimpleInterface as SimpleObject).Test,
                    test4 = a.SimpleInterface.Test,
                    test5 = a.SimpleInterface.TestArray
                })
            });

            Assert.AreEqual(44, response.Data.test.FirstOrDefault().test1);
            Assert.AreEqual(TestEnum.TEST2, response.Data.test.FirstOrDefault().test2);
            Assert.AreEqual(44, response.Data.test.FirstOrDefault().test3);
            Assert.AreEqual(44, response.Data.test.FirstOrDefault().test4);
            Assert.AreEqual(new [] {1, 2, 3}, response.Data.test.FirstOrDefault().test5);
        }

        [Test]
        public void Query_WithFragment_ReturnsCorrectData()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns(@"{ ""data"": { ""field0"": { ""field0"": ""TEST2"" } } }");

            var client = new TestClient(networkClient);

            var data = client.Query(e => new
            {
                test = (e.SimpleInterface as SimpleObject).TestEnum
            });

            Assert.AreEqual(TestEnum.TEST2, data.Data.test);
        }

        [Test]
        public void Query_WithFragmentInsideSelect_ReturnsCorrectData()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns(@"{ ""data"": { ""field0"": { ""field0"": [ { ""field0"": { ""field0"": ""TEST2"",  ""field1"": ""42"",  ""field2"":  [1, 2, 3] } } ] } } }");

            var client = new TestClient(networkClient);

            var data = client.Query(e => new
            {
                testEnum = e.Complex.ComplexArray.Select(a => (a.SimpleInterface as SimpleObject).TestEnum),
                test = e.Complex.ComplexArray.Select(a => (a.SimpleInterface as SimpleObject).Test),
                testArray = e.Complex.ComplexArray.Select(a => (a.SimpleInterface as SimpleObject).TestArray)
            });

            Assert.AreEqual(TestEnum.TEST2, data.Data.testEnum.First());
            Assert.AreEqual(42, data.Data.test.First());
            Assert.AreEqual(new[] { 1, 2, 3 }, data.Data.testArray.First());
        }

        [Test]
        public void Query_WithSimpleObject_ShouldExpandSelectionListToFields()
        {
            var networkClient = Substitute.For<INetworkClient>();
            var client = new TestClient(networkClient);

            var query = client.CreateQuery(e => new
            {
                test = e.Object
            });

            AssertUtils.AreEqualIgnoreLineBreaks(@"query Query {
  field0: object{
    test
    date
    testEnum
    testArray
    __typename
  }
  __typename
}", query.Query);
        }

        [Test]
        public void Query_WithSimpleObjectArray_ShouldExpandSelectionListToFields()
        {
            var networkClient = Substitute.For<INetworkClient>();
            var client = new TestClient(networkClient);

            var query = client.CreateQuery(e => new
            {
                test = e.ObjectArray
            });

            AssertUtils.AreEqualIgnoreLineBreaks(@"query Query {
  field0: objectArray{
    test
    date
    testEnum
    testArray
    __typename
  }
  __typename
}", query.Query);
        }

        [Test]
        public void Query_WithSimpleObject_ShouldGetCorrectData()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns(@"{ ""data"": { ""field0"": { 
  ""test"": 42,
  ""date"": ""11-11-2006"",
  ""testEnum"": ""TEST2"",
  ""testArray"": [1, 2, 3]
} } }");
            var client = new TestClient(networkClient);

            var result = client.Query(e => new
            {
                test = e.Object
            });

            Assert.AreEqual(42, result.Data.test.Test);
            Assert.AreEqual(DateTime.Parse("11-11-2006"), result.Data.test.Date);
            Assert.AreEqual(TestEnum.TEST2, result.Data.test.TestEnum);
            Assert.AreEqual(new [] { 1, 2, 3 }, result.Data.test.TestArray);
        }

        [Test]
        public void Query_WithSimpleObjectArray_ShouldGetCorrectData()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns(@"{ ""data"": { ""field0"": [{ 
  ""test"": 42,
  ""date"": ""11-11-2006"",
  ""testEnum"": ""TEST2"",
  ""testArray"": [1, 2, 3]
}] } }");
            var client = new TestClient(networkClient);

            var result = client.Query(e => new
            {
                test = e.ObjectArray
            });

            Assert.AreEqual(42, result.Data.test.Single().Test);
            Assert.AreEqual(DateTime.Parse("11-11-2006"), result.Data.test.Single().Date);
            Assert.AreEqual(TestEnum.TEST2, result.Data.test.Single().TestEnum);
            Assert.AreEqual(new[] { 1, 2, 3 }, result.Data.test.Single().TestArray);
        }

        [Test]
        public void Query_WithComplexObject_ShouldExpandSelectionListToFields()
        {
            var networkClient = Substitute.For<INetworkClient>();
            var client = new TestClient(networkClient);

            var query = client.CreateQuery(e => new
            {
                test = e.Complex
            });

            AssertUtils.AreEqualIgnoreLineBreaks(@"query Query {
  field0: complex{
    simpleInterface{
      test
      testArray
      __typename
    }
    test
    simple{
      test
      date
      testEnum
      testArray
      __typename
    }
    simpleArray{
      test
      date
      testEnum
      testArray
      __typename
    }
    __typename
  }
  __typename
}", query.Query);
        }

        [Test]
        public void Query_WithComplexObject_ShouldReturnCorrectData()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns(@"{ ""data"": {
  ""field0"": {
    ""simpleInterface"": {
      ""__typename"": ""SimpleObject"",
      ""test"": 41
    },
    ""test"": 42,
    ""simple"": {
      ""test"": 43,
      ""date"": ""11-11-2010"",
      ""testEnum"": ""TEST2"",
      ""testArray"": [1, 2, 3]
    },
    ""simpleArray"": [
        {
          ""test"": 44,
          ""date"": ""11-11-2012"",
          ""testEnum"": ""TEST2"",
          ""testArray"": [4, 5, 6]
        },
        {
          ""test"": 45,
          ""date"": ""11-11-2014"",
          ""testEnum"": ""TEST1"",
          ""testArray"": [7, 8, 9]
        }
    ]
  }
} }");
            var client = new TestClient(networkClient);

            var result = client.Query(e => new
            {
                test = e.Complex
            });

            Assert.AreEqual(41, result.Data.test.SimpleInterface.Test);
            Assert.AreEqual(42, result.Data.test.Test);
            Assert.AreEqual(43, result.Data.test.Simple.Test);
            Assert.AreEqual(DateTime.Parse("11-11-2010"), result.Data.test.Simple.Date);
            Assert.AreEqual(TestEnum.TEST2, result.Data.test.Simple.TestEnum);
            Assert.AreEqual(new [] { 1, 2, 3 }, result.Data.test.Simple.TestArray);

            Assert.AreEqual(44, result.Data.test.SimpleArray.ElementAt(0).Test);
            Assert.AreEqual(DateTime.Parse("11-11-2012"), result.Data.test.SimpleArray.ElementAt(0).Date);
            Assert.AreEqual(TestEnum.TEST2, result.Data.test.SimpleArray.ElementAt(0).TestEnum);
            Assert.AreEqual(new[] { 4, 5, 6 }, result.Data.test.SimpleArray.ElementAt(0).TestArray);

            Assert.AreEqual(45, result.Data.test.SimpleArray.ElementAt(1).Test);
            Assert.AreEqual(DateTime.Parse("11-11-2014"), result.Data.test.SimpleArray.ElementAt(1).Date);
            Assert.AreEqual(TestEnum.TEST1, result.Data.test.SimpleArray.ElementAt(1).TestEnum);
            Assert.AreEqual(new[] { 7, 8, 9 }, result.Data.test.SimpleArray.ElementAt(1).TestArray);
        }

        [Test]
        public void Query_WithComplexObjectThatIsNull_ShouldReturnCorrectData()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns(@"{ ""data"": {
  ""field0"": null
} }");
            var client = new TestClient(networkClient);

            var result = client.Query(e => new
            {
                test = e.Complex
            });

            Assert.IsNull(result.Data.test);
        }

        [Test]
        public void Query_WithComplexObjectThatHasNullFields_ShouldReturnCorrectData()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns(@"{ ""data"": {
  ""field0"": {
    ""simpleInterface"": null,
    ""test"": 42,
    ""simple"": null,
    ""simpleArray"": null
  }
} }");
            var client = new TestClient(networkClient);

            var result = client.Query(e => new
            {
                test = e.Complex
            });

            Assert.IsNull(result.Data.test.SimpleInterface);
            Assert.IsNull(result.Data.test.Simple);
            Assert.IsNull(result.Data.test.SimpleArray);
            Assert.AreEqual(42, result.Data.test.Test);
        }

        [Test]
        public void Query_WithSimpleObjectArrayAndSelectMethod_ShouldExpandSelectionListToFields()
        {
            var networkClient = Substitute.For<INetworkClient>();
            var client = new TestClient(networkClient);

            var query = client.CreateQuery(e => new
            {
                test = e.ObjectArray.Select(o => o)
            });

            AssertUtils.AreEqualIgnoreLineBreaks(@"query Query {
  field0: objectArray{
    test
    date
    testEnum
    testArray
    __typename
  }
  __typename
}", query.Query);
        }

        [Test]
        public void Query_WithSimpleObjectArrayAndNestedSelectMethod_ShouldExpandSelectionListToFields()
        {
            var networkClient = Substitute.For<INetworkClient>();
            var client = new TestClient(networkClient);

            var query = client.CreateQuery(e => new
            {
                test = e.Complex.ComplexArray.Select(o => o.SimpleArray)
            });

            AssertUtils.AreEqualIgnoreLineBreaks(@"query Query {
  field0: complex{
    field0: complexArray{
      field0: simpleArray{
        test
        date
        testEnum
        testArray
        __typename
      }
      __typename
    }
    __typename
  }
  __typename
}", query.Query);
        }

        [Test]
        public void Query_WithSimpleObjectArrayAndMultipleNestedSelectMethod_ShouldExpandSelectionListToFields()
        {
            var networkClient = Substitute.For<INetworkClient>();
            var client = new TestClient(networkClient);

            var query = client.CreateQuery(e => new
            {
                test = e.Complex.ComplexArray.Select(o => new
                {
                    arr = o.SimpleArray,
                    arr2 = o.ComplexArray
                })
            });

            AssertUtils.AreEqualIgnoreLineBreaks(@"query Query {
  field0: complex{
    field0: complexArray{
      field0: simpleArray{
        test
        date
        testEnum
        testArray
        __typename
      }
      field1: complexArray{
        simpleInterface{
          test
          testArray
          __typename
        }
        test
        simple{
          test
          date
          testEnum
          testArray
          __typename
        }
        simpleArray{
          test
          date
          testEnum
          testArray
          __typename
        }
        __typename
      }
      __typename
    }
    __typename
  }
  __typename
}", query.Query);
        }

        [Test]
        public void Query_WithSimpleObjectArrayAndMultipleNestedSelectMethod_ShouldParseDataCorrectly()
        {
            var networkClient = Substitute.For<INetworkClient>();
            networkClient.Send(Arg.Any<GraphQLQueryInfo>()).Returns(@"{ ""data"": {
  ""field0"": {
    ""field0"": [{
      ""field0"": [{
        ""test"": 42,
        ""date"": ""11-11-2020"",
        ""testEnum"": ""TEST2"",
        ""testArray"": [1, 2, 3],
      }],
      ""field1"": [{
        ""simpleInterface"": {
          ""test"": 43,
          ""__typename"": ""SimpleObject""
        },
        ""test"": 11,
        ""simple"": {
          ""test"": 42,
          ""date"": ""11-11-2020"",
          ""testEnum"": ""TEST2"",
          ""testArray"": [1, 2, 3],
        },
        ""simpleArray"": [{
          ""test"": 42,
          ""date"": ""11-11-2020"",
          ""testEnum"": ""TEST2"",
          ""testArray"": [1, 2, 3],
        }]
      }]
    }]
  }
}}");
            var client = new TestClient(networkClient);

            var result = client.Query(e => new
            {
                test = e.Complex.ComplexArray.Select(o => new
                {
                    arr = o.SimpleArray,
                    arr2 = o.ComplexArray
                })
            });

            Assert.IsNotNull(result.Data);
            Assert.IsNotNull(result.Data.test);
            Assert.IsNotNull(result.Data.test.Single().arr);
            Assert.IsNotNull(result.Data.test.Single().arr2);
        }

        [GraphQLType("TestEnum")]
        private enum TestEnum
        {
            TEST1,
            TEST2
        }

        [GraphQLType("SimpleInterface")]
        private interface SimpleInterface
        {
            [GraphQLField("test", "Int!")]
            int Test { get; set; }

            [GraphQLField("testArray", "Int!")]
            IEnumerable<int> TestArray { get; set; }
        }

        [GraphQLType("TestQuery")]
        private class TestQuery
        {
            [GraphQLField("simpleInterface", "SimpleInterface")]
            public SimpleInterface SimpleInterface { get; set; }

            [GraphQLField("testEnum", "TestEnum")]
            public TestEnum TestEnum { get; set; }

            [GraphQLField("test", "Int!")]
            public int Test { get; set; }

            [GraphQLField("date", "Date")]
            public DateTime? Date { get; set; }

            [GraphQLField("object", "SimpleObject")]
            public SimpleObject Object { get; set; }

            [GraphQLField("objectArray", "[SimpleObject]")]
            public IEnumerable<SimpleObject> ObjectArray { get; set; }

            [GraphQLField("complex", "ComplexObject")]
            public ComplexObject Complex { get; set; }

            [GraphQLField("complexWithParams", "ComplexObject")]
            public ComplexObject ComplexWithParams([GraphQLArgument("name", "String!")] string name) { throw new InvalidOperationException(); }
        }

        [GraphQLType("SimpleObject")]
        private class SimpleObject : SimpleInterface
        {
            [GraphQLField("test", "Int")]
            public int Test { get; set; }

            [GraphQLField("date", "Date")]
            public DateTime? Date { get; set; }

            [GraphQLField("testEnum", "TestEnum")]
            public TestEnum TestEnum { get; set; }

            [GraphQLField("testWithParams", "TestWithParams")]
            public int TestWithParams([GraphQLArgument("x", "Float!")] float x) { throw new InvalidOperationException(); }

            [GraphQLField("testArray", "[Int!]")]
            public IEnumerable<int> TestArray { get; set; }
        }

        [GraphQLType("ComplexObject")]
        private class ComplexObject
        {
            [GraphQLField("simpleInterface", "SimpleInterface")]
            public SimpleInterface SimpleInterface { get; set; }

            [GraphQLField("test", "Int")]
            public int Test { get; set; }

            [GraphQLField("simple", "SimpleObject")]
            public SimpleObject Simple { get; set; }

            [GraphQLField("simpleArray", "[SimpleObject]")]
            public IEnumerable<SimpleObject> SimpleArray { get; set; }

            [GraphQLField("complex", "ComplexObject")]
            public ComplexObject Complex { get; set; }

            [GraphQLField("complexWithParams", "ComplexObject")]
            public ComplexObject ComplexWithParams([GraphQLArgument("name", "String!")] string name) { throw new InvalidOperationException(); }

            [GraphQLField("complexWithParams2", "ComplexObject")]
            public ComplexObject ComplexWithParams2([GraphQLArgument("name", "String!")] string name, [GraphQLArgument("surname", "String!")] string surname) { throw new InvalidOperationException(); }

            [GraphQLField("complexArray", "[ComplexObject]")]
            public IEnumerable<ComplexObject> ComplexArray { get; set; }

            [GraphQLField("complexArrayWithparams", "[ComplexObject]")]
            public IEnumerable<ComplexObject> ComplexArrayWithParams([GraphQLArgument("param", "String")] string param) { throw new InvalidOperationException(); }
        }

        private class TestClient : GraphQLCLient<TestQuery>
        {
            public TestClient(INetworkClient client) : base(client)
            {
            }
        }
    }
}
