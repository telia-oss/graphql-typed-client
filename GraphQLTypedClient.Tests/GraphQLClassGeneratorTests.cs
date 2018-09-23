using GraphQLTypedClient.ClassGenerator;
using NUnit.Framework;
using System.IO;

namespace GraphQLTypedClient.Tests
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
                File.ReadAllText("./CodeGenerationCases/SingleTypeWithScalarProps_schema.txt"), "Test");

            Assert.AreEqual(
                File.ReadAllText("./CodeGenerationCases/SingleTypeWithScalarProps_classes.txt"), code);
        }

        [Test]
        public void Generate_SingleEnumType_GeneratesEnum()
        {
            var code = this.generator.Generate(
                File.ReadAllText("./CodeGenerationCases/SingleEnumType_schema.txt"), "Test");

            Assert.AreEqual(
                File.ReadAllText("./CodeGenerationCases/SingleEnumType_classes.txt"), code);
        }

        [Test]
        public void Generate_SingleTypeWithParametersInProps_GeneratesQueryClass()
        {
            var code = this.generator.Generate(
                File.ReadAllText("./CodeGenerationCases/SingleTypeWithParametersInProps_schema.txt"), "Test");

            Assert.AreEqual(
                File.ReadAllText("./CodeGenerationCases/SingleTypeWithParametersInProps_classes.txt"), code);
        }
    }
}