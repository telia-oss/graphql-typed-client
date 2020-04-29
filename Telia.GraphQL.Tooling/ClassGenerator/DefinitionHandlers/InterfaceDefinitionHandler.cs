using System.Collections.Generic;
using System.Linq;
using GraphQLParser.AST;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Telia.GraphQL.Client;

namespace Telia.GraphQL.Tooling.CodeGenerator.DefinitionHandlers
{
    public class InterfaceDefinitionHandler : TypeDefinitionHandlerBase
    {
        public InterfaceDefinitionHandler(GeneratorConfig config) : base(config)
        {
        }

        public override NamespaceDeclarationSyntax Handle(ASTNode definition, NamespaceDeclarationSyntax @namespace, IEnumerable<ASTNode> allDefinitions)
        {
            var objectTypeDefinition = definition as GraphQLInterfaceTypeDefinition;

            var interfaceDeclaration = SyntaxFactory.InterfaceDeclaration(objectTypeDefinition.Name.Value)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddAttributeLists(GetTypeAttributes(objectTypeDefinition.Name.Value));

            interfaceDeclaration = this.CreateProperties(interfaceDeclaration, objectTypeDefinition.Fields, allDefinitions);

            return @namespace.AddMembers(interfaceDeclaration);
        }

        private InterfaceDeclarationSyntax CreateProperties(
            InterfaceDeclarationSyntax interfaceDeclaration, IEnumerable<GraphQLFieldDefinition> fields, IEnumerable<ASTNode> allDefinitions)
        {
            foreach (var field in fields)
            {
                if (field.Arguments == null || field.Arguments.Count() == 0)
                {
                    interfaceDeclaration = GenerateProperty(interfaceDeclaration, field, allDefinitions);
                }
                else
                {
                    interfaceDeclaration = GenerateMethod(interfaceDeclaration, field, allDefinitions);
                }
            }

            return interfaceDeclaration;
        }

        private InterfaceDeclarationSyntax GenerateMethod(
            InterfaceDeclarationSyntax interfaceDeclaration, GraphQLFieldDefinition field, IEnumerable<ASTNode> allDefinitions)
        {
            var returnType = this.GetCSharpTypeFromGraphQLType(field.Type, allDefinitions);
            var methodName = Utils.ToPascalCase(field.Name.Value);

            var method = SyntaxFactory.MethodDeclaration(returnType, methodName)
                .AddAttributeLists(GetFieldAttributes(field.Name.Value))
                .WithParameterList(this.GetParameterList(field.Arguments, allDefinitions))
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

            return interfaceDeclaration.AddMembers(method);
        }

        private InterfaceDeclarationSyntax GenerateProperty(
            InterfaceDeclarationSyntax interfaceDeclaration, GraphQLFieldDefinition field, IEnumerable<ASTNode> allDefinitions)
        {
            var member = SyntaxFactory.PropertyDeclaration(
                this.GetCSharpTypeFromGraphQLType(field.Type, allDefinitions),
                Utils.ToPascalCase(field.Name.Value))
                .AddAttributeLists(GetFieldAttributes(field.Name.Value))
                .AddAccessorListAccessors(
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

            return interfaceDeclaration.AddMembers(member);
        }
    }
}
