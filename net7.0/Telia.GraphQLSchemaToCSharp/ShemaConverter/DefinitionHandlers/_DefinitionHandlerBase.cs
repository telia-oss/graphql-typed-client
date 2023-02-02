using System.Collections;

using GraphQLParser;
using GraphQLParser.AST;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Telia.GraphQLSchemaToCSharp.Config;

namespace Telia.GraphQLSchemaToCSharp.DefinitionHandlers;

internal abstract class DefinitionHandlerBase : IDefinitionHandler
{
    protected ConverterConfig config;

    static string[] ArgumentNameBlackList = new[]
    {
        "abstract",
        "as",
        "base",
        "bool",
        "break",
        "byte",
        "case",
        "catch",
        "char",
        "checked",
        "class",
        "const",
        "continue",
        "decimal",
        "default",
        "delegate",
        "do",
        "double",
        "else",
        "enum",
        "event",
        "explicit",
        "extern",
        "false",
        "finally",
        "fixed",
        "float",
        "for",
        "foreach",
        "goto",
        "if",
        "implicit",
        "in",
        "int",
        "interface",
        "internal",
        "is",
        "is not",
        "lock",
        "long",
        "namespace",
        "new",
        "null",
        "object",
        "operator",
        "out",
        "override",
        "params",
        "private",
        "protected",
        "public",
        "readonly",
        "ref",
        "return",
        "sbyte",
        "sealed",
        "short",
        "sizeof",
        "stackalloc",
        "static",
        "string",
        "struct",
        "switch",
        "this",
        "throw",
        "true",
        "try",
        "typeof",
        "uint",
        "ulong",
        "unchecked",
        "unsafe",
        "ushort",
        "using",
        "virtual",
        "void",
        "volatile",
        "add",
        "alias",
        "ascending",
        "async",
        "await",
        "by",
        "descending",
        "dynamic",
        "equals",
        "from",
        "get",
        "global",
        "group",
        "into",
        "join",
        "let",
        "nameof",
        "on",
        "orderby",
        "partial",
        "remove",
        "select",
        "set",
        "unmanaged",
        "value",
        "var",
        "when",
        "where",
        "where",
        "yield"
    };

    public DefinitionHandlerBase(ConverterConfig config)
    {
        this.config = config;
    }

    public abstract NamespaceDeclarationSyntax Handle(ASTNode definition, NamespaceDeclarationSyntax @namespace, IEnumerable<ASTNode> allDefinitions);

    protected AttributeListSyntax GetFieldAttributes(GraphQLFieldDefinition field)
    {
        var printer = new Printer();
        var attributeArguments = SyntaxFactory.SeparatedList(new[] {
            SyntaxFactory.AttributeArgument(SyntaxFactory.ParseExpression($"\"{field.Name.Value}\"")),
            SyntaxFactory.AttributeArgument(SyntaxFactory.ParseExpression($"\"{printer.Print(field.Type)}\""))});

        var attribute = SyntaxFactory.Attribute(
            SyntaxFactory.ParseName(ShemaConverter.FieldAttributeType.Name),
            SyntaxFactory.AttributeArgumentList(attributeArguments));

        return SyntaxFactory.AttributeList(
            SyntaxFactory.SingletonSeparatedList(attribute));
    }

    protected AttributeListSyntax GetTypeAttributes(string typeName)
    {
        var attributeArguments = SyntaxFactory.SingletonSeparatedList(
            SyntaxFactory.AttributeArgument(SyntaxFactory.ParseExpression($"\"{typeName}\"")));

        var attribute = SyntaxFactory.Attribute(
            SyntaxFactory.ParseName(ShemaConverter.TypeAttributeType.Name),
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
        var cSharpType = this.config.GetCSharpTypeFromGraphQLType(type.Name.Value.Span.ToString(), nullable);

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
                ? SyntaxFactory.NullableType(SyntaxFactory.ParseTypeName(type.Name.Value.Span.ToString()))
                : SyntaxFactory.ParseTypeName(type.Name.Value.Span.ToString());
        }

        return SyntaxFactory.ParseTypeName(type.Name.Value.Span.ToString());
    }

    protected ParameterListSyntax GetParameterList(IEnumerable<GraphQLInputValueDefinition> arguments, IEnumerable<ASTNode> allDefinitions)
    {
        var parameterList = SyntaxFactory.ParameterList();
        var parameters = new List<ParameterSyntax>();

        foreach (var arg in arguments)
        {
            var parameterType = this.GetCSharpTypeFromGraphQLType(arg.Type, allDefinitions);

            var name = ArgumentNameBlackList.Contains(arg.Name.Value.Span.ToString())
                ? $"{arg.Name.Value}Argument"
                : arg.Name.Value.Span.ToString();



            var parameter = SyntaxFactory.Parameter(
                SyntaxFactory.Identifier(name))
                .AddAttributeLists(GetArgumentAttributes(arg))
                .WithType(parameterType);

            // Defaults can't be used in expression so we'll skip them for now
            //if (arg.DefaultValue != null || arg.Type.Kind != ASTNodeKind.NonNullType)
            //{
            //    parameter = parameter.WithDefault(this.GetDefault(arg.DefaultValue, parameterType));
            //}

            parameters.Add(parameter);
        }

        // Optional parameters can appear only after non-optionals
        parameters = parameters.OrderBy(e => e.Default != null).ToList();

        parameterList = parameterList.AddParameters(parameters.ToArray());

        return parameterList;
    }

    protected AttributeListSyntax GetArgumentAttributes(GraphQLInputValueDefinition inputValueDefinition)
    {
        var printer = new Printer();
        var attributeArguments = SyntaxFactory.SeparatedList(new [] {
            SyntaxFactory.AttributeArgument(SyntaxFactory.ParseExpression($"\"{inputValueDefinition.Name.Value}\"")),
            SyntaxFactory.AttributeArgument(SyntaxFactory.ParseExpression($"\"{printer.Print(inputValueDefinition.Type)}\""))});

        var attribute = SyntaxFactory.Attribute(
            SyntaxFactory.ParseName(ShemaConverter.ArgumentAttributeType.Name),
            SyntaxFactory.AttributeArgumentList(attributeArguments));

        return SyntaxFactory.AttributeList(
            SyntaxFactory.SingletonSeparatedList(attribute));
    }
}
