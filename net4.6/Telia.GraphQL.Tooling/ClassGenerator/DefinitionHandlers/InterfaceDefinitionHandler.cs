using System.Collections.Generic;
using System.Linq;
using GraphQLParser.AST;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Telia.GraphQL.Client;

namespace Telia.GraphQL.Tooling.CodeGenerator.DefinitionHandlers
{
    public class InterfaceDefinitionHandler : TypeDefinitionHandlerBase
    {
        public InterfaceDefinitionHandler(GeneratorConfig config) : base(config)
        {
        }

        public override NamespaceDeclarationSyntax Handle(ASTNode definition, NamespaceDeclarationSyntax @namespace, IEnumerable<ASTNode> allDefinitions)
        {
            var objectTypeDefinition = definition as GraphQLInterfaceTypeDefinition;

            var interfaceDeclaration = SyntaxFactory.InterfaceDeclaration(objectTypeDefinition.Name.Value)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddAttributeLists(GetTypeAttributes(objectTypeDefinition.Name.Value));

            interfaceDeclaration = this.CreateProperties(
                objectTypeDefinition.Name.Value,
                interfaceDeclaration,
                objectTypeDefinition.Fields,
                allDefinitions);

            return @namespace.AddMembers(interfaceDeclaration);
        }

        private InterfaceDeclarationSyntax CreateProperties(
            string interfaceTypeName,
            InterfaceDeclarationSyntax interfaceDeclaration,
            IEnumerable<GraphQLFieldDefinition> fields,
            IEnumerable<ASTNode> allDefinitions)
        {
            foreach (var field in fields)
            {
                if (field.Arguments == null || field.Arguments.Count() == 0)
                {
                    interfaceDeclaration = GenerateProperty(interfaceTypeName, interfaceDeclaration, field, allDefinitions);
                }
                else
                {
                    interfaceDeclaration = GenerateMethod(interfaceTypeName, interfaceDeclaration, field, allDefinitions);
                }
            }

            return interfaceDeclaration;
        }

        private InterfaceDeclarationSyntax GenerateMethod(
            string interfaceTypeName,
            InterfaceDeclarationSyntax interfaceDeclaration,
            GraphQLFieldDefinition field,
            IEnumerable<ASTNode> allDefinitions)
        {
            var returnType = this.GetCSharpTypeFromGraphQLType(field.Type, allDefinitions);
            var methodName = PickFieldName(interfaceTypeName, field, allDefinitions);

            var nonNullArguments = field.Arguments.Where(arg => arg.Type.Kind == ASTNodeKind.NonNullType);
            var nullableArguments = field.Arguments.Where(arg => arg.Type.Kind != ASTNodeKind.NonNullType);

            var argumentList = nonNullArguments.ToList();

            interfaceDeclaration = CreateMethod(
                    interfaceDeclaration, field, allDefinitions, returnType, methodName);

            foreach (var argument in nullableArguments)
            {
                argumentList.Add(argument);
                interfaceDeclaration = CreateMethod(
                    interfaceDeclaration, field, allDefinitions, returnType, methodName);
            }

            return interfaceDeclaration;
        }

        private InterfaceDeclarationSyntax CreateMethod(
            InterfaceDeclarationSyntax interfaceDeclaration,
            GraphQLFieldDefinition field,
            IEnumerable<ASTNode> allDefinitions,
            TypeSyntax returnType,
            string methodName)
        {
            var method = SyntaxFactory.MethodDeclaration(returnType, methodName)
                .AddAttributeLists(GetFieldAttributes(field))
                .WithParameterList(this.GetParameterList(field.Arguments, allDefinitions))
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

            return interfaceDeclaration.AddMembers(method);
        }

        private InterfaceDeclarationSyntax GenerateProperty(
            string interfaceTypeName,
            InterfaceDeclarationSyntax interfaceDeclaration,
            GraphQLFieldDefinition field,
            IEnumerable<ASTNode> allDefinitions)
        {
            var member = SyntaxFactory.PropertyDeclaration(
                this.GetCSharpTypeFromGraphQLType(field.Type, allDefinitions),
                PickFieldName(interfaceTypeName, field, allDefinitions))
                .AddAttributeLists(GetFieldAttributes(field))
                .AddAccessorListAccessors(
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

            return interfaceDeclaration.AddMembers(member);
        }

        private string PickFieldName(string interfaceTypeName, GraphQLFieldDefinition field, IEnumerable<ASTNode> allDefinitions)
        {
            var name = Utils.ToPascalCase(field.Name.Value);

            var collidingObjectTypeDefinitions = allDefinitions
                .Where(def => def.Kind == ASTNodeKind.ObjectTypeDefinition)
                .Cast<GraphQLObjectTypeDefinition>()
                .Where(def => def.Name.Value == name &&
                              def.Interfaces != null &&
                              def.Interfaces.Any(i => i.Name.Value == interfaceTypeName));

            if (collidingObjectTypeDefinitions.Any())
            {
                return $"{name}Field";
            }

            return name;
        }
    }
}
