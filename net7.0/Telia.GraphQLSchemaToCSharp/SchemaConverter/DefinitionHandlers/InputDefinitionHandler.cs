using GraphQLParser;
using GraphQLParser.AST;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Telia.GraphQLSchemaToCSharp.Config;

namespace Telia.GraphQLSchemaToCSharp.DefinitionHandlers;

internal class InputDefinitionHandler : DefinitionHandlerBase
{
    public InputDefinitionHandler(ConverterConfig config) : base(config)
    {
    }

    public override NamespaceDeclarationSyntax Handle(ASTNode definition, NamespaceDeclarationSyntax @namespace, IEnumerable<ASTNode> allDefinitions)
    {
        var type = definition as GraphQLInputObjectTypeDefinition;

        var declaration = SyntaxFactory.ClassDeclaration(type.Name.Value.Span.ToString())
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .AddAttributeLists(GetTypeAttributes(type.Name.Value.Span.ToString()));

        declaration = this.CreateMember(
            type.Name.Value.Span.ToString(), declaration, type.Fields, allDefinitions);

        @namespace = @namespace.AddMembers(declaration);

        return @namespace;
    }

    ClassDeclarationSyntax CreateMember(
        string objectTypeName,
        ClassDeclarationSyntax declaration,
        IEnumerable<GraphQLInputValueDefinition> fields,
        IEnumerable<ASTNode> allDefinitions)
    {

        foreach (var field in fields)
        {
            declaration = GenerateProperty(objectTypeName, declaration, field, allDefinitions);
        }

        return declaration;
    }

    ClassDeclarationSyntax GenerateProperty(
        string objectTypeName,
        ClassDeclarationSyntax classDeclaration,
        GraphQLInputValueDefinition property,
        IEnumerable<ASTNode> allDefinitions)
    {
        var member = SyntaxFactory.PropertyDeclaration(
            this.GetCSharpTypeFromGraphQLType(property.Type, allDefinitions),
            PickFieldName(objectTypeName, property))
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.VirtualKeyword))
            .AddAttributeLists(GetPropertyAttributes(property))
            .AddAccessorListAccessors(
                SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

        return classDeclaration.AddMembers(member);
    }

    AttributeListSyntax GetPropertyAttributes(GraphQLInputValueDefinition field)
    {
        var printer = new Printer();
        var attributeArguments = SyntaxFactory.SeparatedList(new[] {
            SyntaxFactory.AttributeArgument(SyntaxFactory.ParseExpression($"\"{field.Name.Value}\"")),
            SyntaxFactory.AttributeArgument(SyntaxFactory.ParseExpression($"\"{printer.Print(field.Type)}\""))});

        var attribute = SyntaxFactory.Attribute(
            SyntaxFactory.ParseName(SchemaConverter.FieldAttributeType.Name),
            SyntaxFactory.AttributeArgumentList(attributeArguments));

        return SyntaxFactory.AttributeList(
            SyntaxFactory.SingletonSeparatedList(attribute));
    }

    string PickFieldName(string objectTypeName, GraphQLInputValueDefinition field)
    {
        var name = Utils.ToPascalCase(field.Name.Value.ToString());

        if (objectTypeName == name)
        {
            return $"{name}Field";
        }

        return name;
    }
}
