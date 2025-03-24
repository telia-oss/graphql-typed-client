using GraphQLParser.AST;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Telia.GraphQLSchemaToCSharp.Config;

namespace Telia.GraphQLSchemaToCSharp.DefinitionHandlers;

internal class TypeDefinitionHandler : DefinitionHandlerBase
{
    public TypeDefinitionHandler(ConverterConfig config) : base(config)
    {
    }

    public override NamespaceDeclarationSyntax Handle(ASTNode definition, NamespaceDeclarationSyntax @namespace, IEnumerable<ASTNode> allDefinitions)
    {
        var type = definition as GraphQLObjectTypeDefinition;

        var declaration = SyntaxFactory.ClassDeclaration(type.Name.Value.Span.ToString())
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .AddAttributeLists(GetTypeAttributes(type.Name.Value.Span.ToString()));

        if (type.Interfaces != null && type.Interfaces.Any())
        {
            var baseList = SyntaxFactory.BaseList();
            foreach (var interfaceImplementation in type.Interfaces)
            {
                baseList = baseList.AddTypes(SyntaxFactory.SimpleBaseType(
                    SyntaxFactory.ParseTypeName(interfaceImplementation.Name.Value.Span.ToString())));
            }

            declaration = declaration.WithBaseList(baseList);
        }

        declaration = this.CreateMember(
            type,
            declaration,
            type.Fields,
            allDefinitions);

        return @namespace.AddMembers(declaration);
    }

    ClassDeclarationSyntax CreateMember(
        GraphQLObjectTypeDefinition objectType,
        ClassDeclarationSyntax declaration,
        IEnumerable<GraphQLFieldDefinition> fields,
        IEnumerable<ASTNode> allDefinitions)
    {
        foreach (var field in fields)
        {
            if (field.Arguments == null || field.Arguments.Count() == 0)
            {
                declaration = GenerateProperty(objectType, declaration, field, allDefinitions);
            }
            else
            {
                declaration = GenerateMethod(objectType, declaration, field, allDefinitions);
            }
        }

        return declaration;
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

        var name = Utils.ToPascalCase(field.Name.Value.Span.ToString());

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
