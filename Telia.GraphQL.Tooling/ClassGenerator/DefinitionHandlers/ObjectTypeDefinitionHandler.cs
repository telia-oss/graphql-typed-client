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
    public class ObjectTypeDefinitionHandler : TypeDefinitionHandlerBase
    {
        public ObjectTypeDefinitionHandler(GeneratorConfig config) : base(config)
        {
        }

        public override NamespaceDeclarationSyntax Handle(ASTNode definition, NamespaceDeclarationSyntax @namespace)
        {
            var objectTypeDefinition = definition as GraphQLObjectTypeDefinition;

            var classDeclaration = SyntaxFactory.ClassDeclaration(objectTypeDefinition.Name.Value)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            if (objectTypeDefinition.Interfaces != null && objectTypeDefinition.Interfaces.Any())
            {
                var baseList = SyntaxFactory.BaseList();
                foreach (var interfaceImplementation in objectTypeDefinition.Interfaces)
                {
                    baseList = baseList.AddTypes(SyntaxFactory.SimpleBaseType(
                        SyntaxFactory.ParseTypeName(interfaceImplementation.Name.Value)));
                }

                classDeclaration = classDeclaration.WithBaseList(baseList);
            }

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
    }
}
