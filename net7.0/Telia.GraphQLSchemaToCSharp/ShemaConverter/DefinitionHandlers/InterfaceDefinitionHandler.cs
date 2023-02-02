using GraphQLParser.AST;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Telia.GraphQLSchemaToCSharp.Config;

namespace Telia.GraphQLSchemaToCSharp.DefinitionHandlers;

internal class InterfaceDefinitionHandler : DefinitionHandlerBase
{
    public InterfaceDefinitionHandler(ConverterConfig config) : base(config)
    {
    }

    public override NamespaceDeclarationSyntax Handle(ASTNode definition, NamespaceDeclarationSyntax @namespace, IEnumerable<ASTNode> allDefinitions)
    {
        var type = definition as GraphQLInterfaceTypeDefinition;

        var declaration = SyntaxFactory.InterfaceDeclaration(type.Name.Value.Span.ToString())
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .AddAttributeLists(GetTypeAttributes(type.Name.Value.Span.ToString()));

        declaration = this.CreateMember(
            type.Name.Value.Span.ToString(),
            declaration,
            type.Fields,
            allDefinitions);

        return @namespace.AddMembers(declaration);
    }

    InterfaceDeclarationSyntax CreateMember(
        string interfaceTypeName,
        InterfaceDeclarationSyntax declaration,
        IEnumerable<GraphQLFieldDefinition> fields,
        IEnumerable<ASTNode> allDefinitions)
    {
        foreach (var field in fields)
        {
            if (field.Arguments == null || field.Arguments.Count() == 0)
            {
                declaration = GenerateProperty(interfaceTypeName, declaration, field, allDefinitions);
            }
            else
            {
                declaration = GenerateMethod(interfaceTypeName, declaration, field, allDefinitions);
            }
        }

        return declaration;
    }

    InterfaceDeclarationSyntax GenerateMethod(
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

    InterfaceDeclarationSyntax CreateMethod(
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

    InterfaceDeclarationSyntax GenerateProperty(
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

    string PickFieldName(string interfaceTypeName, GraphQLFieldDefinition field, IEnumerable<ASTNode> allDefinitions)
    {
        var name = Utils.ToPascalCase(field.Name.Value.Span.ToString());

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
