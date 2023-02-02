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

            classDeclaration = this.CreateProperties(
                objectTypeDefinition,
                classDeclaration,
                objectTypeDefinition.Fields,
                allDefinitions);

            return @namespace.AddMembers(classDeclaration);
        }

        ClassDeclarationSyntax CreateProperties(
            GraphQLObjectTypeDefinition objectType,
            ClassDeclarationSyntax classDeclaration,
            IEnumerable<GraphQLFieldDefinition> fields,
            IEnumerable<ASTNode> allDefinitions)
        {
            foreach (var field in fields)
            {
                if (field.Arguments == null || field.Arguments.Count() == 0)
                {
                    classDeclaration = GenerateProperty(objectType, classDeclaration, field, allDefinitions);
                }
                else
                {
                    classDeclaration = GenerateMethod(objectType, classDeclaration, field, allDefinitions);
                }
            }

            return classDeclaration;
        }

        ClassDeclarationSyntax GenerateMethod(
            GraphQLObjectTypeDefinition objectType,
            ClassDeclarationSyntax classDeclaration,
            GraphQLFieldDefinition field,
            IEnumerable<ASTNode> allDefinitions)
        {
            var returnType = this.GetCSharpTypeFromGraphQLType(field.Type, allDefinitions);
            var methodName = PickFieldName(objectType, field, allDefinitions);

            var nonNullArguments = field.Arguments.Where(arg => arg.Type.Kind == ASTNodeKind.NonNullType);
            var nullableArguments = field.Arguments.Where(arg => arg.Type.Kind != ASTNodeKind.NonNullType);

            var argumentList = nonNullArguments.ToList();

            classDeclaration = CreateMethod(
                    classDeclaration, field, allDefinitions, returnType, methodName, argumentList);

            foreach (var argument in nullableArguments)
            {
                argumentList.Add(argument);
                classDeclaration = CreateMethod(
                    classDeclaration, field, allDefinitions, returnType, methodName, argumentList);
            }

            return classDeclaration;
        }

        ClassDeclarationSyntax CreateMethod(
            ClassDeclarationSyntax classDeclaration,
            GraphQLFieldDefinition field,
            IEnumerable<ASTNode> allDefinitions,
            TypeSyntax returnType,
            string methodName,
            List<GraphQLInputValueDefinition> argumentList)
        {
            var method = SyntaxFactory.MethodDeclaration(returnType, methodName)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.VirtualKeyword))
                .AddAttributeLists(GetFieldAttributes(field))
                .WithParameterList(this.GetParameterList(argumentList, allDefinitions))
                .WithBody(this.GetEmptyBody());

            return classDeclaration.AddMembers(method);
        }

        BlockSyntax GetEmptyBody()
        {
            var throwException = SyntaxFactory.ThrowStatement(
                    SyntaxFactory.InvocationExpression(
                        SyntaxFactory.IdentifierName("new InvalidOperationException")));

            return SyntaxFactory.Block(throwException);
        }

        ClassDeclarationSyntax GenerateProperty(
            GraphQLObjectTypeDefinition objectType,
            ClassDeclarationSyntax classDeclaration,
            GraphQLFieldDefinition field,
            IEnumerable<ASTNode> allDefinitions)
        {
            var member = SyntaxFactory.PropertyDeclaration(
                this.GetCSharpTypeFromGraphQLType(field.Type, allDefinitions),
                PickFieldName(objectType, field, allDefinitions))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.VirtualKeyword))
                .AddAttributeLists(GetFieldAttributes(field))
                .AddAccessorListAccessors(
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

            return classDeclaration.AddMembers(member);
        }

        string PickFieldName(GraphQLObjectTypeDefinition objectType, GraphQLFieldDefinition field, IEnumerable<ASTNode> allDefinitions)
        {
            var implementedInterfaceDefinitions = allDefinitions
                .Where(def => def.Kind == ASTNodeKind.InterfaceTypeDefinition)
                .Cast<GraphQLInterfaceTypeDefinition>()
                .Where(def => objectType?.Interfaces != null && objectType.Interfaces.Any(e => e.Name.Value == def.Name.Value));

            var name = Utils.ToPascalCase(field.Name.Value);

            if (objectType.Name.Value == name)
            {
                return $"{name}Field";
            }

            foreach (var interfaceDefinition in implementedInterfaceDefinitions)
            {
                var collidingObjectTypeDefinitions = allDefinitions
                    .Where(def => def.Kind == ASTNodeKind.ObjectTypeDefinition)
                    .Cast<GraphQLObjectTypeDefinition>()
                    .Where(def => def.Name.Value == name &&
                                  def.Interfaces != null &&
                                  def.Interfaces.Any(i => i.Name.Value == interfaceDefinition.Name.Value));

                if (collidingObjectTypeDefinitions.Any())
                {
                    return $"{name}Field";
                }
            }

            return name;
        }
    }
}
