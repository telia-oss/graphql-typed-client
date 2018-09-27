using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace Telia.GraphQL.Tooling.CodeGenerator
{
    public class GeneratorConfig
    {
        private Dictionary<string, Type> graphQLToCSharpTypeBindings;

        public GeneratorConfig()
        {
            this.SetupDefaults();
        }

        private void SetupDefaults()
        {
            this.graphQLToCSharpTypeBindings = new Dictionary<string, Type>();

            this.graphQLToCSharpTypeBindings.Add("Int", typeof(Int32));
            this.graphQLToCSharpTypeBindings.Add("Float", typeof(Single));
            this.graphQLToCSharpTypeBindings.Add("String", typeof(String));
            this.graphQLToCSharpTypeBindings.Add("Boolean", typeof(Boolean));
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
