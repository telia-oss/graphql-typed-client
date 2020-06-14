using System;
using System.Collections;
using System.Collections.Generic;
using GraphQLParser.AST;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Telia.GraphQL.Client;

namespace Telia.GraphQL.Tooling.CodeGenerator.DefinitionHandlers
{
    public class InputObjectTypeDefinitionHandler : TypeDefinitionHandlerBase
    {
        private GeneratorConfig config;

        public InputObjectTypeDefinitionHandler(GeneratorConfig config) : base(config)
        {
            this.config = config;
        }

        public override NamespaceDeclarationSyntax Handle(ASTNode definition, NamespaceDeclarationSyntax @namespace, IEnumerable<ASTNode> allDefinitions)
        {
            var objectTypeDefinition = definition as GraphQLInputObjectTypeDefinition;

            var classDeclaration = SyntaxFactory.ClassDeclaration(objectTypeDefinition.Name.Value)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddAttributeLists(GetTypeAttributes(objectTypeDefinition.Name.Value));

            classDeclaration = this.CreateProperties(
                objectTypeDefinition.Name.Value, classDeclaration, objectTypeDefinition.Fields, allDefinitions);

            return @namespace.AddMembers(classDeclaration);
        }

        private ClassDeclarationSyntax CreateProperties(
            string objectTypeName,
            ClassDeclarationSyntax classDeclaration,
            IEnumerable<GraphQLInputValueDefinition> fields,
            IEnumerable<ASTNode> allDefinitions)
        {
            foreach (var field in fields)
            {
                classDeclaration = GenerateProperty(objectTypeName, classDeclaration, field, allDefinitions);
            }

            return classDeclaration;
        }

        private ClassDeclarationSyntax GenerateProperty(
            string objectTypeName,
            ClassDeclarationSyntax classDeclaration,
            GraphQLInputValueDefinition field,
            IEnumerable<ASTNode> allDefinitions)
        {
            var member = SyntaxFactory.PropertyDeclaration(
                this.GetCSharpTypeFromGraphQLType(field.Type, allDefinitions),
                PickFieldName(objectTypeName, field))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.VirtualKeyword))
                .AddAttributeLists(GetFieldAttributes(field.Name.Value))
                .AddAccessorListAccessors(
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

            return classDeclaration.AddMembers(member);
        }

        private AttributeListSyntax GetFieldAttributes(string fieldName)
        {
            var attributeArguments = SyntaxFactory.SingletonSeparatedList(
                SyntaxFactory.AttributeArgument(SyntaxFactory.ParseExpression($"\"{fieldName}\"")));

            var attribute = SyntaxFactory.Attribute(
                SyntaxFactory.ParseName("GraphQLField"),
                SyntaxFactory.AttributeArgumentList(attributeArguments));

            return SyntaxFactory.AttributeList(
                SyntaxFactory.SingletonSeparatedList(attribute));
        }

        private string PickFieldName(string objectTypeName, GraphQLInputValueDefinition field)
        {
            var name = Utils.ToPascalCase(field.Name.Value);

            if (objectTypeName == name)
            {
                return $"{name}Field";
            }

            return name;
        }
    }
}
