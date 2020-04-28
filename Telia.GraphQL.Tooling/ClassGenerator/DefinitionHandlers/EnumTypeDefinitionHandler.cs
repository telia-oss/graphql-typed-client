using GraphQLParser.AST;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Telia.GraphQL.Tooling.CodeGenerator.DefinitionHandlers
{
    public class EnumTypeDefinitionHandler : TypeDefinitionHandlerBase
    {
        public EnumTypeDefinitionHandler() : base(null)
        {
        }

        public override NamespaceDeclarationSyntax Handle(ASTNode definition, NamespaceDeclarationSyntax @namespace)
        {
            var enumTypeDefinition = definition as GraphQLEnumTypeDefinition;

            var enumDeclaration = SyntaxFactory.EnumDeclaration(enumTypeDefinition.Name.Value)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddAttributeLists(GetTypeAttributes(enumTypeDefinition.Name.Value));

            foreach (var value in enumTypeDefinition.Values)
            {
                enumDeclaration = enumDeclaration.AddMembers(SyntaxFactory.EnumMemberDeclaration(value.Name.Value));
            }

            return @namespace.AddMembers(enumDeclaration);
        }
    }
}
