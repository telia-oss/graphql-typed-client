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

            classDeclaration = this.CreateProperties(classDeclaration, objectTypeDefinition.Fields);

            return @namespace.AddMembers(classDeclaration);
        }

        private ClassDeclarationSyntax CreateProperties(
            ClassDeclarationSyntax classDeclaration, IEnumerable<GraphQLInputValueDefinition> fields)
        {
            foreach (var field in fields)
            {
                classDeclaration = GenerateProperty(classDeclaration, field);
            }

            return classDeclaration;
        }

        private ClassDeclarationSyntax GenerateProperty(
            ClassDeclarationSyntax classDeclaration, GraphQLInputValueDefinition field)
        {
            var member = SyntaxFactory.PropertyDeclaration(
                this.GetCSharpTypeFromGraphQLType(field.Type),
                Utils.ToPascalCase(field.Name.Value))
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

        private TypeSyntax GetCSharpTypeFromGraphQLType(GraphQLType type)
        {
            switch (type.Kind)
            {
                case ASTNodeKind.NamedType: return this.GetCSharpTypeFromGraphQLNamedType((GraphQLNamedType)type);
                case ASTNodeKind.NonNullType: return this.GetCSharpTypeFromGraphQLNonNullType((GraphQLNonNullType)type);
                case ASTNodeKind.ListType: return this.GetCSharpTypeFromGraphQLListType((GraphQLListType)type);
            }

            throw new NotImplementedException($"GetCSharpTypeFromGraphQLType: Unknown type.Kind: {type.Kind}");
        }

        private TypeSyntax GetCSharpTypeFromGraphQLListType(GraphQLListType type)
        {
            var underlyingType = this.GetCSharpTypeFromGraphQLType(type.Type);

            var argumentList = SyntaxFactory.TypeArgumentList(
                SyntaxFactory.SeparatedList<TypeSyntax>()
                    .Add(underlyingType));

            return SyntaxFactory.GenericName(SyntaxFactory.Identifier(typeof(IEnumerable).Name), argumentList);
        }

        private TypeSyntax GetCSharpTypeFromGraphQLNonNullType(GraphQLNonNullType type)
        {
            switch (type.Type.Kind)
            {
                case ASTNodeKind.NamedType: return this.GetCSharpTypeFromGraphQLNamedType((GraphQLNamedType)type.Type, false);
                case ASTNodeKind.ListType: return this.GetCSharpTypeFromGraphQLListType((GraphQLListType)type.Type);
            }

            throw new NotImplementedException($"GetCSharpTypeFromGraphQLNonNullType: Unknown type.Kind: {type.Type.Kind}");
        }

        private TypeSyntax GetCSharpTypeFromGraphQLNamedType(GraphQLNamedType type, bool nullable = true)
        {
            var cSharpType = this.config.GetCSharpTypeFromGraphQLType(type.Name.Value, nullable);

            if (cSharpType == null)
            {
                return SyntaxFactory.ParseTypeName(type.Name.Value);
            }

            return cSharpType;
        }
    }
}
