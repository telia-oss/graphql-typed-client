﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework;

using Telia.GraphQLSchemaToCSharp.Tests.Attributes;

namespace Telia.GraphQLSchemaToCSharp.Tests;

[TestClass]
public class FullSchemaConverterTests
{
    [TestMethod]
    public void Convert_Simple_GraphQl_Schema_To_Models_Success()
    {
        /*
            NOTE: the simple graphql is pushed to git
         */
        var graphqlFileData = Assemblies.GetEmbeddedResource("_LocalTestFiles/ModelsSimple.graphql");

        if (graphqlFileData.IsNot())
        {
            // NOTE 2: File is empty, copy your own ModelsSimple.graphql content to the file and rerun
            return;
        }

        var converter = new SchemaConverter();

        var code = converter.Convert<GraphQLTypeAttribute, GraphQLFieldAttribute, GraphQLArgumentAttribute>(graphqlFileData, "Test");

        var folder = AppDomain.CurrentDomain.BaseDirectory;
        var directory = new DirectoryInfo(folder).Parent.Parent.Parent;

        var outputPath = directory.FullName + "\\_LocalTestFiles\\Output.cs";

        File.WriteAllText(outputPath, code);

        Assert.IsTrue(code != null && code.Length > 100, "Output was empty");

        Assert.IsTrue(code.Contains("namespace "), "missing namespaces");

        Assert.IsTrue(code.Contains("public class "), "missing public class");

        Assert.IsTrue(code.Contains(nameof(GraphQLFieldAttribute)), "missing GraphQLFieldAttribute");

        // NOTE: Recompile after test run to verify that output is compilable
    }

    [TestMethod]
    public void Convert_GraphQl_Schema_To_Models_Success()
    {
        /*
            NOTE: .graphql file is not pushed to github
            Simply use your own instead
            
            NOTE 2: .graphql file should be of some size
            So attributes like GraphQlArgument, Fields, Type etc are all used, to bypass the poor mans tests below

            NOTE 3: verify final output by recompiling this very project after 'test run', it will then recompile with the output file
         */
        var graphqlFileData = Assemblies.GetEmbeddedResource("_LocalTestFiles/Models.graphql");

        if(graphqlFileData.IsNot())
        {
            // NOTE 4: File is empty, copy your own models.graphql content to the file and rerun
            return;
        }

        var converter = new SchemaConverter();

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
