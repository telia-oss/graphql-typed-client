using System.Collections.Generic;
using GraphQLParser.AST;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Telia.GraphQL.Tooling.CodeGenerator.DefinitionHandlers
{
    public interface IDefinitionHandler
    {
        NamespaceDeclarationSyntax Handle(ASTNode definition, NamespaceDeclarationSyntax @namespace, IEnumerable<ASTNode> allDefinitons);
    }
}
