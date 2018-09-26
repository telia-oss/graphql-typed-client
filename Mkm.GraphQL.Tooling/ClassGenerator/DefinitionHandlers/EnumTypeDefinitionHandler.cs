using GraphQLParser.AST;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mkm.GraphQL.Tooling.CodeGenerator.DefinitionHandlers
{
    public class EnumTypeDefinitionHandler : IDefinitionHandler
    {
        public NamespaceDeclarationSyntax Handle(ASTNode definition, NamespaceDeclarationSyntax @namespace)
        {
            var enumTypeDefinition = definition as GraphQLEnumTypeDefinition;

            var enumDeclaration = SyntaxFactory.EnumDeclaration(enumTypeDefinition.Name.Value)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            foreach (var value in enumTypeDefinition.Values)
            {
                enumDeclaration = enumDeclaration.AddMembers(SyntaxFactory.EnumMemberDeclaration(value.Name.Value));
            }

            return @namespace.AddMembers(enumDeclaration);
        }
    }
}
