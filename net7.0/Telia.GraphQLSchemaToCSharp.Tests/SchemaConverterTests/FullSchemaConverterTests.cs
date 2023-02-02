using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Net;

using Telia.GraphQLSchemaToCSharp.Tests.Attributes;

namespace Telia.GraphQLSchemaToCSharp.Tests;

[TestClass]
public class FullSchemaConverterTests
{
    [TestMethod]
    public void Convert_Simple_Schema_To_Models_Success()
    {
        //NOTE: File is not pushed to github, find the proper repository and copy paste it to the "_LocalTestFiles" folder
        var graphqlFileData = Assemblies.GetEmbeddedResource("_LocalTestFiles", "Models.graphql");

        var converter = new ShemaConverter();

        var code = converter.Convert<GraphQLTypeAttribute, GraphQLFieldAttribute, GraphQLArgumentAttribute>(graphqlFileData, "Test");

        var folder = AppDomain.CurrentDomain.BaseDirectory;
        var directory = new DirectoryInfo(folder).Parent.Parent.Parent;
        
        var outputPath = directory.FullName + "\\_LocalTestFiles\\Output.cs";

        File.WriteAllText(outputPath, code);

        Assert.IsTrue(code != null && code.Length > 100, "Output was empty");

        Assert.IsTrue(code.Contains("namespace "), "missing namespaces");

        Assert.IsTrue(code.Contains("public class "), "missing public class");

        Assert.IsTrue(code.Contains(nameof(GraphQLTypeAttribute)), "missing GraphQLTypeAttribute");

        Assert.IsTrue(code.Contains(nameof(GraphQLFieldAttribute)), "missing GraphQLFieldAttribute");

        Assert.IsTrue(code.Contains(nameof(GraphQLArgumentAttribute)), "missing GraphQLArgumentAttribute");

        // NOTE: Recompile after test run to verify that output is compilable
    }
}
