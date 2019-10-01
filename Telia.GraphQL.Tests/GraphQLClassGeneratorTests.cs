using Telia.GraphQL.Tooling.CodeGenerator;
using NUnit.Framework;
using System.IO;

namespace Telia.GraphQL.Tests
{
    [TestFixture]
    public class GraphQLClassGeneratorTests
    {
        private Generator generator;

        [SetUp]
        public void Setup()
        {
            this.generator = new Generator();
        }

        [Test]
        public void Generate_SingleTypeWithScalarProps_GeneratesQueryClass()
        {
            var code = this.generator.Generate(
                LoadData("./CodeGenerationCases/SingleTypeWithScalarProps_schema.txt"), "Test");

            AssertUtils.AreEqualIgnoreLineBreaks(
                LoadData("./CodeGenerationCases/SingleTypeWithScalarProps_classes.txt"), code);
        }

        [Test]
        public void Generate_SingleEnumType_GeneratesEnum()
        {
            var code = this.generator.Generate(
                LoadData("CodeGenerationCases/SingleEnumType_schema.txt"), "Test");

            AssertUtils.AreEqualIgnoreLineBreaks(
                LoadData("CodeGenerationCases/SingleEnumType_classes.txt"), code);
        }

        [Test]
        public void Generate_SingleTypeWithParametersInProps_GeneratesQueryClass()
        {
            var code = this.generator.Generate(
                LoadData("./CodeGenerationCases/SingleTypeWithParametersInProps_schema.txt"), "Test");

            AssertUtils.AreEqualIgnoreLineBreaks(
                LoadData("./CodeGenerationCases/SingleTypeWithParametersInProps_classes.txt"), code);
        }

        [Test]
        public void Generate_InputObjectType_GeneratesClass()
        {
            var code = this.generator.Generate(
                LoadData("./CodeGenerationCases/InputObjectType_schema.txt"), "Test");

            AssertUtils.AreEqualIgnoreLineBreaks(
                LoadData("./CodeGenerationCases/InputObjectType_classes.txt"), code);
        }

        [Test]
        public void Generate_NestedInputObjectType_GeneratesClasses()
        {
            var code = this.generator.Generate(
                LoadData("./CodeGenerationCases/NestedInputObjectType_schema.txt"), "Test");

            AssertUtils.AreEqualIgnoreLineBreaks(
                LoadData("./CodeGenerationCases/NestedInputObjectType_classes.txt"), code);
        }

        [Test]
        public void Generate_SingleTypeWithAWSTypes_GeneratesClasses()
        {
            var code = this.generator.Generate(
                LoadData("./CodeGenerationCases/SingleTypeWithAWSTypes_schema.txt"), "Test");

            AssertUtils.AreEqualIgnoreLineBreaks(
                LoadData("./CodeGenerationCases/SingleTypeWithAWSTypes_classes.txt"), code);
        }

        [Test]
        public void Generate_InterfaceWithImplementation_GeneratesClasses()
        {
            var code = this.generator.Generate(
                LoadData("./CodeGenerationCases/InterfaceWithImplementation_schema.txt"), "Test");

            AssertUtils.AreEqualIgnoreLineBreaks(
                LoadData("./CodeGenerationCases/InterfaceWithImplementation_classes.txt"), code);
        }

        private static string LoadData(string filename)
        {
            return File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, filename));
        }
    }
}