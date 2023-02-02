using GraphQLParser;
using GraphQLParser.AST;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Telia.GraphQLSchemaToCSharp.Config;
using Telia.GraphQLSchemaToCSharp.DefinitionHandlers;

namespace Telia.GraphQLSchemaToCSharp;

public class ShemaConverter
{
    IDictionary<ASTNodeKind, IDefinitionHandler> handlers;

    internal static Type TypeAttributeType { get; set; }
    internal static Type FieldAttributeType { get; set; }
    internal static Type ArgumentAttributeType { get; set; }

    public ShemaConverter(ConverterConfig config = null)
    {
        if (config == null)
            config = new ConverterConfig();

        this.handlers = new Dictionary<ASTNodeKind, IDefinitionHandler>
        {
            { ASTNodeKind.ObjectTypeDefinition, new TypeDefinitionHandler(config) },
            { ASTNodeKind.EnumTypeDefinition, new EnumTypeDefinitionHandler() },
            { ASTNodeKind.InputObjectTypeDefinition, new InputDefinitionHandler(config) },
            { ASTNodeKind.InterfaceTypeDefinition, new InterfaceDefinitionHandler(config) },
            { ASTNodeKind.SchemaDefinition, new SchemaDefinitionHandler(config) }
        };
    }

    public string Convert<TGraphQLTypeAttribute, TGraphQLFieldAttribute, TGraphQlArgumentAttribute>(string graphQLSchemaData, string schemaNamespace)
    {
        TypeAttributeType = typeof(TGraphQLTypeAttribute);
        FieldAttributeType = typeof(TGraphQLFieldAttribute);
        ArgumentAttributeType = typeof(TGraphQlArgumentAttribute);

        var options = new ParserOptions();

        options.Ignore = IgnoreOptions.All;

        var syntaxTree = Parser.Parse(graphQLSchemaData, options);

        var @namespace = GenerateNamespace(schemaNamespace);

        @namespace = GenerateTypeDefinitions(syntaxTree.Definitions, @namespace);

        return @namespace
            .NormalizeWhitespace()
            .ToFullString();
    }

    NamespaceDeclarationSyntax GenerateTypeDefinitions(
        IEnumerable<ASTNode> definitions, NamespaceDeclarationSyntax @namespace)
    {
        foreach (var definition in definitions)
        {
            if (handlers.ContainsKey(definition.Kind))
            {
                @namespace = handlers[definition.Kind].Handle(definition, @namespace, definitions);
            }
        }

        return @namespace;
    }

    NamespaceDeclarationSyntax GenerateNamespace(string schemaName)
    {
        var namespaceName = SyntaxFactory.ParseName(schemaName);
        var @namespace = SyntaxFactory.NamespaceDeclaration(namespaceName).NormalizeWhitespace();

        var namespaces = new List<string>();
        
        namespaces.Add("System");
        namespaces.Add("System.Collections.Generic");

        namespaces.Add(TypeAttributeType.Namespace);

        if(!namespaces.Contains(FieldAttributeType.Namespace))
            namespaces.Add(FieldAttributeType.Namespace);

        if (!namespaces.Contains(ArgumentAttributeType.Namespace))
            namespaces.Add(ArgumentAttributeType.Namespace);

        foreach (var ns in namespaces)
            if (ns != null && ns != "")
                @namespace = @namespace.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(ns)));

        return @namespace;
    }
}
