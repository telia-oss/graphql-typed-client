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

        public override NamespaceDeclarationSyntax Handle(ASTNode definition, NamespaceDeclarationSyntax @namespace, IEnumerable<ASTNode> allDefinitions)
        {
            var objectTypeDefinition = definition as GraphQLObjectTypeDefinition;

            var classDeclaration = SyntaxFactory.ClassDeclaration(objectTypeDefinition.Name.Value)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddAttributeLists(GetTypeAttributes(objectTypeDefinition.Name.Value));

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

            classDeclaration = this.CreateProperties(classDeclaration, objectTypeDefinition.Fields, allDefinitions);

            return @namespace.AddMembers(classDeclaration);
        }

        private ClassDeclarationSyntax CreateProperties(
            ClassDeclarationSyntax classDeclaration, IEnumerable<GraphQLFieldDefinition> fields, IEnumerable<ASTNode> allDefinitions)
        {
            foreach (var field in fields)
            {
                if (field.Arguments == null || field.Arguments.Count() == 0)
                {
                    classDeclaration = GenerateProperty(classDeclaration, field, allDefinitions);
                }
                else
                {
                    classDeclaration = GenerateMethod(classDeclaration, field, allDefinitions);
                }
            }

            return classDeclaration;
        }

        private ClassDeclarationSyntax GenerateMethod(ClassDeclarationSyntax classDeclaration, GraphQLFieldDefinition field, IEnumerable<ASTNode> allDefinitions)
        {
            var returnType = this.GetCSharpTypeFromGraphQLType(field.Type, allDefinitions);
            var methodName = Utils.ToPascalCase(field.Name.Value);

            var method = SyntaxFactory.MethodDeclaration(returnType, methodName)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.VirtualKeyword))
                .AddAttributeLists(GetFieldAttributes(field.Name.Value))
                .WithParameterList(this.GetParameterList(field.Arguments, allDefinitions))
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

        private ClassDeclarationSyntax GenerateProperty(ClassDeclarationSyntax classDeclaration, GraphQLFieldDefinition field, IEnumerable<ASTNode> allDefinitions)
        {
            var member = SyntaxFactory.PropertyDeclaration(
                this.GetCSharpTypeFromGraphQLType(field.Type, allDefinitions),
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
    }
}
