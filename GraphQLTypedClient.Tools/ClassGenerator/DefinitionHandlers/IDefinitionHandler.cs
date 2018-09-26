using GraphQLParser.AST;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GraphQLTypedClient.ClassGenerator.DefinitionHandlers
{
    public interface IDefinitionHandler
    {
        NamespaceDeclarationSyntax Handle(ASTNode definition, NamespaceDeclarationSyntax @namespace);
    }
}
