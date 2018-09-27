using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GraphQLParser.AST;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Telia.GraphQL.Client;

namespace Telia.GraphQL.Tooling.CodeGenerator.DefinitionHandlers
{
    public class ObjectTypeDefinitionHandler : IDefinitionHandler
    {
        private GeneratorConfig config;

        public ObjectTypeDefinitionHandler(GeneratorConfig config)
        {
            this.config = config;
        }

        public NamespaceDeclarationSyntax Handle(ASTNode definition, NamespaceDeclarationSyntax @namespace)
        {
            var objectTypeDefinition = definition as GraphQLObjectTypeDefinition;

            var classDeclaration = SyntaxFactory.ClassDeclaration(objectTypeDefinition.Name.Value)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            classDeclaration = this.CreateProperties(classDeclaration, objectTypeDefinition.Fields);

            return @namespace.AddMembers(classDeclaration);
        }

        private ClassDeclarationSyntax CreateProperties(
            ClassDeclarationSyntax classDeclaration, IEnumerable<GraphQLFieldDefinition> fields)
        {
            foreach (var field in fields)
            {
                if (field.Arguments == null || field.Arguments.Count() == 0)
                {
                    classDeclaration = GenerateProperty(classDeclaration, field);
                }
                else
                {
                    classDeclaration = GenerateMethod(classDeclaration, field);
                }
            }

            return classDeclaration;
        }

        private ClassDeclarationSyntax GenerateMethod(ClassDeclarationSyntax classDeclaration, GraphQLFieldDefinition field)
        {
            var returnType = this.GetCSharpTypeFromGraphQLType(field.Type);
            var methodName = Utils.ToPascalCase(field.Name.Value);

            var method = SyntaxFactory.MethodDeclaration(returnType, methodName)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddAttributeLists(GetFieldAttributes(field.Name.Value))
                .WithParameterList(this.GetParameterList(field.Arguments))
                .WithBody(this.GetEmptyBody());

            return classDeclaration.AddMembers(method);
        }

        private BlockSyntax GetEmptyBody()
        {
            var throwException = SyntaxFactory.ThrowStatement(
                    SyntaxFactory.InvocationExpression(
                        SyntaxFactory.IdentifierName("new InvalidOperationException")));

            return SyntaxFactory.Block(throwException);
        }

        private ParameterListSyntax GetParameterList(IEnumerable<GraphQLInputValueDefinition> arguments)
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

        private EqualsValueClauseSyntax GetDefault(GraphQLValue defaultValue)
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

        private ClassDeclarationSyntax GenerateProperty(ClassDeclarationSyntax classDeclaration, GraphQLFieldDefinition field)
        {
            var member = SyntaxFactory.PropertyDeclaration(
                this.GetCSharpTypeFromGraphQLType(field.Type),
                Utils.ToPascalCase(field.Name.Value))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
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

            throw new NotImplementedException();
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

            throw new NotImplementedException();
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
