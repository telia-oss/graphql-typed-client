using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Net;

using Telia.GraphQL;
using Telia.LinqToGraphQLToModel.Tests._Abstract;
using Telia.LinqToGraphQLToModel.Tests.LinqToGraphql.Schema;

namespace Telia.LinqToGraphQLToModel.Tests.LinqToGraphql.Tests;

[TestClass]
public class DummySchemaTests : BaseTestClass
{
    [TestMethod]
    public void Query_With_Scalar()
    {
        var q = new GraphQLQuery<DummySchema>();

        var graphql = q.Query(e => new
        {
            test = e.Test,
            test2 = e.Object.TestWithParams(0.5f)
        });

        var expected = GetExpected(nameof(Query_With_Scalar));

        IsEqualIgnoreWhitespace(graphql, expected);
    }

    [TestMethod]
    public void Query_Object_With_Properties()
    {
        var q = new GraphQLQuery<DummySchema>();

        var graphql = q.Query(e => new
        {
            o = new
            {
                b = e.Object.Test
            }
        });

        var expected = GetExpected(nameof(Query_Object_With_Properties));

        IsEqualIgnoreWhitespace(graphql, expected);
    }

    [TestMethod]
    public void Query_Object_With_Multiple_Child_Objects()
    {
        var q = new GraphQLQuery<DummySchema>();

        var graphql = q.Query(e => new
        {
            o = e.Complex.Test,
            b = e.Complex.Complex.Complex.Test,
            c = e.Complex.Complex.Simple.Test
        });

        var expected = GetExpected(nameof(Query_Object_With_Multiple_Child_Objects));

        IsEqualIgnoreWhitespace(graphql, expected);
    }

    [TestMethod]
    public void Query_Object_With_Multiple_Child_Objects_With_Multiple_Variables()
    {
        string var1 = "test1";
        string var2 = "test2";
        string var3 = "test3";
        string var4 = "test4";

        var q = new GraphQLQuery<DummySchema>();

        var graphql = q.Query(e => new
        {
            o = e.ComplexWithParams(var1).Test,
            b = e.ComplexWithParams(var4).Complex.ComplexWithParams(var2).Complex.Test,
            c = e.ComplexWithParams(var1).ComplexWithParams2(var3, var4).Simple.Test
        });

        var expected = GetExpected(nameof(Query_Object_With_Multiple_Child_Objects_With_Multiple_Variables));

        IsEqualIgnoreWhitespace(graphql, expected);
    }

    [TestMethod]
    public void Query_Object_With_Multiple_Child_Objects_With_Multiple_Inline_Variables()
    {
        var q = new GraphQLQuery<DummySchema>();

        var graphql = q.Query(e => new
        {
            o = e.ComplexWithParams(null).Test,
            b = e.ComplexWithParams("test4").Complex.ComplexWithParams("test2").Complex.Test,
            c = e.ComplexWithParams("test1").ComplexWithParams("test3").Simple.Test
        });
        
        var expected = GetExpected(nameof(Query_Object_With_Multiple_Child_Objects_With_Multiple_Inline_Variables));

        IsEqualIgnoreWhitespace(graphql, expected);
    }


    [TestMethod]
    public void Query_Object_With_Multiple_Nested_Child_Objects_With_Inline_Variables()
    {
        var q = new GraphQLQuery<DummySchema>();

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

        var graphql = q.Query(e => new
        {
            o = e.ComplexWithParams(input.test1).Test,
            b = e.ComplexWithParams(input.others.others.others.test4).Complex.ComplexWithParams(input.others.test2).Complex.Test,
            c = e.ComplexWithParams(input.test1).ComplexWithParams(input.others.others.test3).Simple.Test
        });

        var expected = GetExpected(nameof(Query_Object_With_Multiple_Nested_Child_Objects_With_Inline_Variables));

        IsEqualIgnoreWhitespace(graphql, expected);
    }

    [TestMethod]
    public void Query_Select_Scalar_Array()
    {
        var q = new GraphQLQuery<DummySchema>();

        var graphql = q.Query(e => new
        {
            o = e.Complex.ComplexArray.Select(x => x.Test)
        });

        var expected = GetExpected(nameof(Query_Select_Scalar_Array));

        IsEqualIgnoreWhitespace(graphql, expected);
    }

    [TestMethod]
    public void Query_Select_List_Of_Anonymous_Type()
    {
        var q = new GraphQLQuery<DummySchema>();

        var graphql = q.Query(e => new
        {
            o = e.Complex.ComplexArray.Select(x => new { a = x.Test, b = e.Test })
        });

        var expected = GetExpected(nameof(Query_Select_List_Of_Anonymous_Type));

        IsEqualIgnoreWhitespace(graphql, expected);
    }

    [TestMethod]
    public void Query_Select_Multiple_One_Scalar()
    {
        var q = new GraphQLQuery<DummySchema>();

        var graphql = q.Query(e => new
        {
            test = e.Test,
            test2 = e.Test,
            test3 = e.Test
        });

        var expected = GetExpected(nameof(Query_Select_Multiple_One_Scalar));

        IsEqualIgnoreWhitespace(graphql, expected);
    }

    string GetExpected(string fileName)
    {
        return Assemblies.GetEmbeddedResource("LinqToGraphql/Assets", fileName + ".txt");
    }
}
