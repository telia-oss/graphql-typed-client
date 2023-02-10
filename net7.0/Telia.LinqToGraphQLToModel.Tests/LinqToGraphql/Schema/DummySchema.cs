using Telia.LinqToGraphQLToModel.Schema.Attributes;

namespace Telia.LinqToGraphQLToModel.Tests.LinqToGraphql.Schema;

[GraphQLType("TestEnum")]
enum TestEnum
{
    TEST1,
    TEST2
}

[GraphQLType("SimpleInterface")]
interface SimpleInterface
{
    [GraphQLField("test", "Int!")]
    int Test { get; set; }

    [GraphQLField("testArray", "Int!")]
    IEnumerable<int> TestArray { get; set; }
}

[GraphQLType("Query")]
class DummySchema
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
class SimpleObject : SimpleInterface
{
    [GraphQLField("test", "Int")]
    public int Test { get; set; }

    [GraphQLField("date", "Date")]
    public DateTime? Date { get; set; }

    [GraphQLField("testEnum", "TestEnum")]
    public TestEnum TestEnum { get; set; }

    [GraphQLField("testEnumFilter", "TestEnumFilter")]
    public TestEnum TestEnumFilter([GraphQLArgument("x", "TestEnum!")] TestEnum x) { throw new InvalidOperationException(); }

    [GraphQLField("testWithParams", "TestWithParams")]
    public int TestWithParams([GraphQLArgument("x", "Float!")] float x) { throw new InvalidOperationException(); }

    [GraphQLField("testArray", "[Int!]")]
    public IEnumerable<int> TestArray { get; set; }
}

[GraphQLType("ComplexObject")]
class ComplexObject
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
