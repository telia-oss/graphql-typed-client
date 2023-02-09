using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Net;

using Telia.GraphQLSchemaToCSharp.Tests.Attributes;

namespace Telia.GraphQLSchemaToCSharp.Tests;

[TestClass]
public class SchemaConverterTests
{
    string GetTextFromFile(string file)
    {
        return Assemblies.GetEmbeddedResource("_TestFiles", file);
    }

    string GetCode(string schemaFileName)
    {
        var graphqlFileData = GetTextFromFile(schemaFileName);

        var converter = new SchemaConverter();

        return converter.Convert<GraphQLTypeAttribute, GraphQLFieldAttribute, GraphQLArgumentAttribute>(graphqlFileData, "Test");
    }

    string GetExpected(string classFileName)
    {
        return GetTextFromFile(classFileName);
    }

    [TestMethod]
    public void InputType_Is_Converted_Success()
    {
        var code = GetCode("InputType_schema.txt");

        var expected = GetExpected("InputType_classes.txt");

        IsEqualIgnoreWhitespace(code, expected);
    }

    [TestMethod]
    public void InterfaceWithImplementation_Is_Converted_Success()
    {
        var code = GetCode("InterfaceWithImplementation_schema.txt");

        var expected = GetExpected("InterfaceWithImplementation_classes.txt");

        IsEqualIgnoreWhitespace(code, expected);
    }

    [TestMethod]
    public void NestedInputObjectType_Is_Converted_Success()
    {
        var code = GetCode("NestedInputObjectType_schema.txt");

        var expected = GetExpected("NestedInputObjectType_classes.txt");

        IsEqualIgnoreWhitespace(code, expected);
    }

    [TestMethod]
    public void SingleEnumType_Is_Converted_Success()
    {
        var code = GetCode("SingleEnumType_schema.txt");

        var expected = GetExpected("SingleEnumType_classes.txt");

        IsEqualIgnoreWhitespace(code, expected);
    }

    [TestMethod]
    public void SingleTypeWithAWSTypes_Is_Converted_Success()
    {
        var code = GetCode("SingleTypeWithAWSTypes_schema.txt");

        var expected = GetExpected("SingleTypeWithAWSTypes_classes.txt");

        IsEqualIgnoreWhitespace(code, expected);
    }

    [TestMethod]
    public void SingleTypeWithParametersInProps_Is_Converted_Success()
    {
        var code = GetCode("SingleTypeWithParametersInProps_schema.txt");

        var expected = GetExpected("SingleTypeWithParametersInProps_classes.txt");

        IsEqualIgnoreWhitespace(code, expected);
    }

    [TestMethod]
    public void SingleTypeWithScalarProps_Is_Converted_Success()
    {
        var code = GetCode("SingleTypeWithScalarProps_schema.txt");

        var expected = GetExpected("SingleTypeWithScalarProps_classes.txt");

        IsEqualIgnoreWhitespace(code, expected);
    }

    [TestMethod]
    public void TypesWithCollidingFieldAndTypeNames_Is_Converted_Success()
    {
        var code = GetCode("TypesWithCollidingFieldAndTypeNames_schema.txt");

        var expected = GetExpected("TypesWithCollidingFieldAndTypeNames_classes.txt");

        IsEqualIgnoreWhitespace(code, expected);
    }

    void IsEqualIgnoreWhitespace(string text, string expected)
    {
        text = Minify(text);

        expected = Minify(expected);
        try
        {
            Assert.IsTrue(text == expected);
        }
        catch
        {
            Dump.Write(text);
            Dump.Write(expected);

            Assert.IsTrue(text == expected);
        }
    }

    string Minify(string data)
    {
        var sb = new StringBuilder(data);

        sb.Replace(Environment.NewLine, " ")
            .Replace("\t", " ")
            .Replace("\\r\\n", " ")
            .Replace("\\n", " ")
            .Replace("  ", " ")
            .Replace("  ", " ")
            .Replace("  ", " ")
            .Replace("  ", " ")
            .Replace("  ", " ")
            .Replace(": ", ":")
            .Replace("{ ", "{")
            .Replace(" }", "}")
            .Replace(", ", ",")
            .Replace("? >", "?>");

        return sb.ToString();
    }
}