using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Telia.GraphQL.Tooling.CodeGenerator
{
    public class GeneratorConfig
    {
        Dictionary<string, Type> graphQLToCSharpTypeBindings;

        public GeneratorConfig()
        {
            this.SetupDefaults();
        }

        void SetupDefaults()
        {
            this.graphQLToCSharpTypeBindings = new Dictionary<string, Type>();

            this.graphQLToCSharpTypeBindings.Add("Int", typeof(Int32));
            this.graphQLToCSharpTypeBindings.Add("Float", typeof(Single));
            this.graphQLToCSharpTypeBindings.Add("Boolean", typeof(Boolean));
            this.graphQLToCSharpTypeBindings.Add("String", typeof(String));
            this.graphQLToCSharpTypeBindings.Add("AWSDate", typeof(DateTime));
            this.graphQLToCSharpTypeBindings.Add("AWSTime", typeof(TimeSpan));
            this.graphQLToCSharpTypeBindings.Add("AWSDateTime", typeof(DateTime));
            this.graphQLToCSharpTypeBindings.Add("AWSTimestamp", typeof(Int32));
            this.graphQLToCSharpTypeBindings.Add("AWSEmail", typeof(string));
            this.graphQLToCSharpTypeBindings.Add("AWSJSON", typeof(string));
            this.graphQLToCSharpTypeBindings.Add("AWSURL", typeof(string));
            this.graphQLToCSharpTypeBindings.Add("AWSPhone", typeof(string));
            this.graphQLToCSharpTypeBindings.Add("AWSIPAddress", typeof(string));
            this.graphQLToCSharpTypeBindings.Add("ID", typeof(string));
            this.graphQLToCSharpTypeBindings.Add("DateTime", typeof(DateTime));
        }

        public void AddOrReplaceTypeBinding(string graphQLType, Type cSharpType)
        {
            if (this.graphQLToCSharpTypeBindings.ContainsKey(graphQLType))
            {
                this.graphQLToCSharpTypeBindings.Remove(graphQLType);
            }

            this.graphQLToCSharpTypeBindings.Add(graphQLType, cSharpType);
        }

        public TypeSyntax GetCSharpTypeFromGraphQLType(string graphQLType, bool nullable)
        {
            if (!this.graphQLToCSharpTypeBindings.ContainsKey(graphQLType))
            {
                return null;
            }

            var type = this.graphQLToCSharpTypeBindings[graphQLType];

            if (type.IsValueType && nullable)
            {
                var typeName = SyntaxFactory.ParseTypeName(type.Name);

                return SyntaxFactory.NullableType(typeName);
            }

            return SyntaxFactory.ParseTypeName(type.Name);
        }
    }
}
