using GraphQLParser.AST;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Telia.GraphQLSchemaToCSharp.Config;

namespace Telia.GraphQLSchemaToCSharp.DefinitionHandlers;

internal class SchemaDefinitionHandler : DefinitionHandlerBase
{
    public SchemaDefinitionHandler(ConverterConfig config) : base(config)
    {
    }

    public override NamespaceDeclarationSyntax Handle(ASTNode definition, NamespaceDeclarationSyntax @namespace, IEnumerable<ASTNode> allDefinitions)
    {
        return @namespace;
    }
}
