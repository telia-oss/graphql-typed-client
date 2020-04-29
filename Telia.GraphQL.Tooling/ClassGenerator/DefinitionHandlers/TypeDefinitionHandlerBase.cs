using GraphQLParser.AST;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Telia.GraphQL.Tooling.CodeGenerator.DefinitionHandlers
{
    public abstract class TypeDefinitionHandlerBase : IDefinitionHandler
    {
        protected GeneratorConfig config;

        public TypeDefinitionHandlerBase(GeneratorConfig config)
        {
            this.config = config;
        }

        public abstract NamespaceDeclarationSyntax Handle(ASTNode definition, NamespaceDeclarationSyntax @namespace, IEnumerable<ASTNode> allDefinitions);

        protected EqualsValueClauseSyntax GetDefault(GraphQLValue defaultValue, TypeSyntax type)
        {
            switch (defaultValue.Kind)
            {
                case ASTNodeKind.IntValue:
                    return SyntaxFactory.EqualsValueClause(
                        SyntaxFactory.LiteralExpression(
                         SyntaxKind.NumericLiteralExpression,
                         SyntaxFactory.ParseToken(((GraphQLScalarValue)defaultValue).Value)));

                case ASTNodeKind.FloatValue:
                    return SyntaxFactory.EqualsValueClause(
                        SyntaxFactory.LiteralExpression(
                            SyntaxKind.NumericLiteralExpression,
                            SyntaxFactory.ParseToken($"{((GraphQLScalarValue)defaultValue).Value}f")));

                case ASTNodeKind.StringValue:
                    return SyntaxFactory.EqualsValueClause(
                        SyntaxFactory.LiteralExpression(
                            SyntaxKind.StringLiteralExpression,
                            SyntaxFactory.ParseToken($"\"{((GraphQLScalarValue)defaultValue).Value}\""))); ;

                case ASTNodeKind.BooleanValue:
                    return ((GraphQLScalarValue)defaultValue).Value.ToLower() == "true"
                        ? SyntaxFactory.EqualsValueClause(SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression))
                        : SyntaxFactory.EqualsValueClause(SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression));

                case ASTNodeKind.EnumValue:
                    return SyntaxFactory.EqualsValueClause(SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        type.Kind() == SyntaxKind.NullableType
                            ? ((NullableTypeSyntax)type).ElementType
                            : type,
                        SyntaxFactory.IdentifierName(((GraphQLScalarValue) defaultValue).Value)));
            };

            return SyntaxFactory.EqualsValueClause(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
        }

        protected AttributeListSyntax GetFieldAttributes(string fieldName)
        {
            var attributeArguments = SyntaxFactory.SingletonSeparatedList(
                SyntaxFactory.AttributeArgument(SyntaxFactory.ParseExpression($"\"{fieldName}\"")));

            var attribute = SyntaxFactory.Attribute(
                SyntaxFactory.ParseName("GraphQLField"),
                SyntaxFactory.AttributeArgumentList(attributeArguments));

            return SyntaxFactory.AttributeList(
                SyntaxFactory.SingletonSeparatedList(attribute));
        }

        protected AttributeListSyntax GetTypeAttributes(string typeName)
        {
            var attributeArguments = SyntaxFactory.SingletonSeparatedList(
                SyntaxFactory.AttributeArgument(SyntaxFactory.ParseExpression($"\"{typeName}\"")));

            var attribute = SyntaxFactory.Attribute(
                SyntaxFactory.ParseName("GraphQLType"),
                SyntaxFactory.AttributeArgumentList(attributeArguments));

            return SyntaxFactory.AttributeList(
                SyntaxFactory.SingletonSeparatedList(attribute));
        }

        protected TypeSyntax GetCSharpTypeFromGraphQLType(GraphQLType type, IEnumerable<ASTNode> allDefinitions)
        {
            switch (type.Kind)
            {
                case ASTNodeKind.NamedType: return this.GetCSharpTypeFromGraphQLNamedType((GraphQLNamedType)type, allDefinitions);
                case ASTNodeKind.NonNullType: return this.GetCSharpTypeFromGraphQLNonNullType((GraphQLNonNullType)type, allDefinitions);
                case ASTNodeKind.ListType: return this.GetCSharpTypeFromGraphQLListType((GraphQLListType)type, allDefinitions);
            }

            throw new NotImplementedException($"GetCSharpTypeFromGraphQLType: Unknown type.Kind: {type.Kind}");
        }

        protected TypeSyntax GetCSharpTypeFromGraphQLListType(GraphQLListType type, IEnumerable<ASTNode> allDefinitions)
        {
            var underlyingType = this.GetCSharpTypeFromGraphQLType(type.Type, allDefinitions);

            var argumentList = SyntaxFactory.TypeArgumentList(
                SyntaxFactory.SeparatedList<TypeSyntax>()
                    .Add(underlyingType));

            return SyntaxFactory.GenericName(SyntaxFactory.Identifier(typeof(IEnumerable).Name), argumentList);
        }

        protected TypeSyntax GetCSharpTypeFromGraphQLNonNullType(GraphQLNonNullType type, IEnumerable<ASTNode> allDefinitions)
        {
            switch (type.Type.Kind)
            {
                case ASTNodeKind.NamedType: return this.GetCSharpTypeFromGraphQLNamedType((GraphQLNamedType)type.Type, allDefinitions, false);
                case ASTNodeKind.ListType: return this.GetCSharpTypeFromGraphQLListType((GraphQLListType)type.Type, allDefinitions);
            }

            throw new NotImplementedException($"GetCSharpTypeFromGraphQLNonNullType: Unknown type.Kind: {type.Type.Kind}");
        }

        protected TypeSyntax GetCSharpTypeFromGraphQLNamedType(GraphQLNamedType type, IEnumerable<ASTNode> allDefinitions, bool nullable = true)
        {
            var cSharpType = this.config.GetCSharpTypeFromGraphQLType(type.Name.Value, nullable);

            if (cSharpType != null)
            {
                return cSharpType;
            }

            var enumTypeFromSchema = allDefinitions
                .Where(e => e.Kind == ASTNodeKind.EnumTypeDefinition)
                .Cast<GraphQLEnumTypeDefinition>()
                .SingleOrDefault(e => e.Name.Value == type.Name.Value);

            if (enumTypeFromSchema != null)
            {
                return nullable
                    ? SyntaxFactory.NullableType(SyntaxFactory.ParseTypeName(type.Name.Value))
                    : SyntaxFactory.ParseTypeName(type.Name.Value);
            }

            return SyntaxFactory.ParseTypeName(type.Name.Value);
        }

        protected ParameterListSyntax GetParameterList(IEnumerable<GraphQLInputValueDefinition> arguments, IEnumerable<ASTNode> allDefinitions)
        {
            var parameterList = SyntaxFactory.ParameterList();

            foreach (var arg in arguments)
            {
                var parameterType = this.GetCSharpTypeFromGraphQLType(arg.Type, allDefinitions);

                var parameter = SyntaxFactory.Parameter(
                    SyntaxFactory.Identifier(arg.Name.Value))
                    .WithType(parameterType);

                if (arg.DefaultValue != null)
                {
                    parameter = parameter.WithDefault(this.GetDefault(arg.DefaultValue, parameterType));
                }

                parameterList = parameterList.AddParameters(parameter);
            }

            return parameterList;
        }
    }
}
