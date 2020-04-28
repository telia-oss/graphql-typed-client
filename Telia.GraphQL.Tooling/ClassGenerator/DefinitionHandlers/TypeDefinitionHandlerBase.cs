using GraphQLParser.AST;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Telia.GraphQL.Tooling.CodeGenerator.DefinitionHandlers
{
    public abstract class TypeDefinitionHandlerBase : IDefinitionHandler
    {
        protected GeneratorConfig config;

        public TypeDefinitionHandlerBase(GeneratorConfig config)
        {
            this.config = config;
        }

        public abstract NamespaceDeclarationSyntax Handle(ASTNode definition, NamespaceDeclarationSyntax @namespace);

        protected EqualsValueClauseSyntax GetDefault(GraphQLValue defaultValue)
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

        protected TypeSyntax GetCSharpTypeFromGraphQLType(GraphQLType type)
        {
            switch (type.Kind)
            {
                case ASTNodeKind.NamedType: return this.GetCSharpTypeFromGraphQLNamedType((GraphQLNamedType)type);
                case ASTNodeKind.NonNullType: return this.GetCSharpTypeFromGraphQLNonNullType((GraphQLNonNullType)type);
                case ASTNodeKind.ListType: return this.GetCSharpTypeFromGraphQLListType((GraphQLListType)type);
            }

            throw new NotImplementedException($"GetCSharpTypeFromGraphQLType: Unknown type.Kind: {type.Kind}");
        }

        protected TypeSyntax GetCSharpTypeFromGraphQLListType(GraphQLListType type)
        {
            var underlyingType = this.GetCSharpTypeFromGraphQLType(type.Type);

            var argumentList = SyntaxFactory.TypeArgumentList(
                SyntaxFactory.SeparatedList<TypeSyntax>()
                    .Add(underlyingType));

            return SyntaxFactory.GenericName(SyntaxFactory.Identifier(typeof(IEnumerable).Name), argumentList);
        }

        protected TypeSyntax GetCSharpTypeFromGraphQLNonNullType(GraphQLNonNullType type)
        {
            switch (type.Type.Kind)
            {
                case ASTNodeKind.NamedType: return this.GetCSharpTypeFromGraphQLNamedType((GraphQLNamedType)type.Type, false);
                case ASTNodeKind.ListType: return this.GetCSharpTypeFromGraphQLListType((GraphQLListType)type.Type);
            }

            throw new NotImplementedException($"GetCSharpTypeFromGraphQLNonNullType: Unknown type.Kind: {type.Type.Kind}");
        }

        protected TypeSyntax GetCSharpTypeFromGraphQLNamedType(GraphQLNamedType type, bool nullable = true)
        {
            var cSharpType = this.config.GetCSharpTypeFromGraphQLType(type.Name.Value, nullable);

            if (cSharpType == null)
            {
                return SyntaxFactory.ParseTypeName(type.Name.Value);
            }

            return cSharpType;
        }

        protected ParameterListSyntax GetParameterList(IEnumerable<GraphQLInputValueDefinition> arguments)
        {
            var parameterList = SyntaxFactory.ParameterList();

            foreach (var arg in arguments)
            {
                var parameterType = this.GetCSharpTypeFromGraphQLType(arg.Type);

                var parameter = SyntaxFactory.Parameter(
                    SyntaxFactory.Identifier(arg.Name.Value))
                    .WithType(parameterType);

                if (arg.DefaultValue != null)
                {
                    parameter = parameter.WithDefault(this.GetDefault(arg.DefaultValue));
                }

                parameterList = parameterList.AddParameters(parameter);
            }

            return parameterList;
        }
    }
}
