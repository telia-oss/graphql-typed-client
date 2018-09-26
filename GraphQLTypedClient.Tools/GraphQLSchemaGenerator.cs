using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Samples.VisualStudio.GeneratorSample;
using Microsoft.VisualStudio.Shell;

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
                return Encoding.ASCII.GetBytes("Some cntent");
            }
            catch (Exception e)
            {
                this.GeneratorError(4, e.ToString(), 1, 1);
                return null;
            }
        }
    }
}