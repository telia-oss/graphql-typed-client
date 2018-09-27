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

            Assert.AreEqual(
                LoadData("./CodeGenerationCases/SingleTypeWithScalarProps_classes.txt"), code);
        }

        [Test]
        public void Generate_SingleEnumType_GeneratesEnum()
        {
            var code = this.generator.Generate(
                LoadData("CodeGenerationCases/SingleEnumType_schema.txt"), "Test");

            Assert.AreEqual(
                LoadData("CodeGenerationCases/SingleEnumType_classes.txt"), code);
        }

        [Test]
        public void Generate_SingleTypeWithParametersInProps_GeneratesQueryClass()
        {
            var code = this.generator.Generate(
                LoadData("./CodeGenerationCases/SingleTypeWithParametersInProps_schema.txt"), "Test");

            Assert.AreEqual(
                LoadData("./CodeGenerationCases/SingleTypeWithParametersInProps_classes.txt"), code);
        }

        private static string LoadData(string filename)
        {
            return File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, filename));
        }
    }
}