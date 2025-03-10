using GraphQLParser.AST;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Telia.GraphQLSchemaToCSharp.DefinitionHandlers;

internal interface IDefinitionHandler
{
    NamespaceDeclarationSyntax Handle(ASTNode definition, NamespaceDeclarationSyntax @namespace, IEnumerable<ASTNode> allDefinitons);
}
