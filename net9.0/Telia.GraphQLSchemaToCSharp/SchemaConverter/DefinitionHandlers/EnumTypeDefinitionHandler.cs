using GraphQLParser.AST;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Telia.GraphQLSchemaToCSharp.DefinitionHandlers;

internal class EnumTypeDefinitionHandler : DefinitionHandlerBase
{
    public EnumTypeDefinitionHandler() : base(null)
    {
    }

    public override NamespaceDeclarationSyntax Handle(ASTNode definition, NamespaceDeclarationSyntax @namespace, IEnumerable<ASTNode> allDefinitions)
    {
        var type = definition as GraphQLEnumTypeDefinition;

        var declaration = SyntaxFactory.EnumDeclaration(type.Name.Value.Span.ToString())
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .AddAttributeLists(GetTypeAttributes(type.Name.Value.Span.ToString()));

        foreach (var value in type.Values)
        {
            declaration = declaration.AddMembers(SyntaxFactory.EnumMemberDeclaration(value.Name.Value.Span.ToString()));
        }

        return @namespace.AddMembers(declaration);
    }
}
