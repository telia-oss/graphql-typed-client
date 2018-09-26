using System;
using System.Runtime.InteropServices;
using System.Text;
using Mkm.GraphQL.Tooling;
using Microsoft.VisualStudio.Shell;
using Mkm.GraphQL.Tooling.CodeGenerator;

namespace GraphQLTypedClient.Tools
{
    [ComVisible(true)]
    [Guid("52B316AA-1997-4c81-9969-83604C09EEB4")]
    [CodeGeneratorRegistration(typeof(GraphQLSchemaGenerator), "C# GraphQL Schema generator", "{FAE04EC1-301F-11D3-BF4B-00C04F79EFBC}", GeneratesDesignTimeSource = true)]
    [ProvideObject(typeof(GraphQLSchemaGenerator))]
    public class GraphQLSchemaGenerator : BaseCodeGeneratorWithSite
    {
#pragma warning disable 0414
        //The name of this generator (use for 'Custom Tool' property of project item)
        internal static string name = "GraphQLSchemaGenerator";
#pragma warning restore 0414

        protected override byte[] GenerateCode(string inputFileContent)
        {
            try
            {
                var config = new GeneratorConfig();
                var generator = new Generator(config);
                var code = generator.Generate(inputFileContent, this.FileNameSpace);

                return Encoding.ASCII.GetBytes(code);
            }
            catch (Exception e)
            {
                this.GeneratorError(4, e.ToString(), 1, 1);
                return null;
            }
        }
    }
}