using GraphQLParser;
using GraphQLParser.AST;
using Telia.GraphQL.Tooling.CodeGenerator.DefinitionHandlers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Telia.GraphQL.Tooling.CodeGenerator
{
    public class Generator
    {
        private readonly Parser graphQLParser;
        private readonly GeneratorConfig config;
        private readonly IDictionary<ASTNodeKind, IDefinitionHandler> handlers;

        public Generator(GeneratorConfig config)
        {
            this.graphQLParser = new Parser(new Lexer());
            this.config = config;
            this.handlers = new Dictionary<ASTNodeKind, IDefinitionHandler>
            {
                { ASTNodeKind.ObjectTypeDefinition, new ObjectTypeDefinitionHandler(config) },
                { ASTNodeKind.EnumTypeDefinition, new EnumTypeDefinitionHandler() },
                { ASTNodeKind.InputObjectTypeDefinition, new InputObjectTypeDefinitionHandler(config) },
                { ASTNodeKind.InterfaceTypeDefinition, new InterfaceDefinitionHandler(config) }
            };
        }

        public Generator(): this(new GeneratorConfig())
        {
            
        }

        public string Generate(string graphQLSchema, string schemaName, string classPrefix = "")
        {
            var syntaxTree = this.graphQLParser.Parse(new Source(graphQLSchema));
            var @namespace = GenerateNamespace(schemaName);

            @namespace = this.GenerateTypeDefinitions(syntaxTree.Definitions, @namespace);

            return @namespace
                .NormalizeWhitespace()
                .ToFullString();
        }

        private NamespaceDeclarationSyntax GenerateTypeDefinitions(
            IEnumerable<ASTNode> definitions, NamespaceDeclarationSyntax @namespace)
        {
            foreach (var definition in definitions)
            {
                if (handlers.ContainsKey(definition.Kind))
                {
                    @namespace = handlers[definition.Kind].Handle(definition, @namespace);
                }
            }

            return @namespace;
        }

        private NamespaceDeclarationSyntax GenerateNamespace(string schemaName)
        {
            var namespaceName = SyntaxFactory.ParseName(schemaName);
            var @namespace = SyntaxFactory.NamespaceDeclaration(namespaceName).NormalizeWhitespace();

            // Add System using statement: (using System)
            return @namespace.AddUsings(
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")),
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Collections.Generic")),
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Telia.GraphQL.Client.Attributes"))
            );
        }
    }
}
