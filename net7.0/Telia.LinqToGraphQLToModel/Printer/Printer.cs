using GraphQLParser.AST;

using SystemLibrary.Common.Net.Extensions;

namespace Telia.LinqToGraphQLToModel.GraphQLParser;

public class Printer
{
    /// <summary>
    /// TODO: This uses our "own old" printer instead of the AST Printer/SDL Printer from GraphQLParser as that gave errors, so stopped trying, but should update to it
    /// - Could optimize with stringbuilder and spans ("latest features"), compared to "ROM" object of ours
    /// </summary>
    public string Print(ASTNode node)
    {
        if (node == null)
        {
            return string.Empty;
        }

        return node.Kind switch
        {
            ASTNodeKind.Document => PrintDocument((GraphQLDocument)node),
            ASTNodeKind.OperationDefinition => PrintOperationDefinition((GraphQLOperationDefinition)node),
            ASTNodeKind.SelectionSet => PrintSelectionSet((GraphQLSelectionSet)node),
            ASTNodeKind.Field => PrintFieldNode((GraphQLFieldNode)node),
            ASTNodeKind.Name => PrintName((GraphQLName)node),
            ASTNodeKind.Argument => PrintArgument((GraphQLArgument)node),
            ASTNodeKind.FragmentSpread => PrintFragmentSpread((GraphQLFragmentSpread)node),
            ASTNodeKind.FragmentDefinition => PrintFragmentDefinition((GraphQLFragmentDefinition)node),
            ASTNodeKind.InlineFragment => PrintInlineFragment((GraphQLInlineFragment)node),
            ASTNodeKind.NamedType => PrintNamedType((GraphQLNamedType)node),
            ASTNodeKind.Directive => PrintDirective((GraphQLDirective)node),
            ASTNodeKind.Variable => PrintVariable((GraphQLVariable)node),
            ASTNodeKind.IntValue => PrintIntValue((GraphQLIntValue)node),
            ASTNodeKind.FloatValue => PrintFloatValue((GraphQLFloatValue)node),
            ASTNodeKind.StringValue => PrintStringValue((GraphQLStringValue)node),
            ASTNodeKind.BooleanValue => PrintBooleanValue((GraphQLBooleanValue)node),
            ASTNodeKind.EnumValue => PrintEnumValue((GraphQLEnumValue)node),
            ASTNodeKind.ListValue => PrintListValue((GraphQLListValue)node),
            ASTNodeKind.ObjectValue => PrintObjectValue((GraphQLObjectValue)node),
            ASTNodeKind.ObjectField => PrintObjectField((GraphQLObjectField)node),
            ASTNodeKind.VariableDefinition => PrintVariableDefinition((GraphQLVariableDefinition)node),
            ASTNodeKind.NullValue => PrintNullValue(),
            ASTNodeKind.SchemaDefinition => PrintSchemaDefinition((GraphQLSchemaDefinition)node),
            ASTNodeKind.ListType => PrintListType((GraphQLListType)node),
            ASTNodeKind.NonNullType => PrintNonNullType((GraphQLNonNullType)node),

            ASTNodeKind.ScalarTypeDefinition => PrintScalarTypeDefinition((GraphQLScalarTypeDefinition)node),
            ASTNodeKind.ObjectTypeDefinition => PrintObjectTypeDefinition((GraphQLObjectTypeDefinition)node),
            ASTNodeKind.FieldDefinition => PrintFieldDefinition((GraphQLFieldDefinition)node),
            ASTNodeKind.InputValueDefinition => PrintInputValueDefinition((GraphQLInputValueDefinition)node),
            ASTNodeKind.InterfaceTypeDefinition => PrintInterfaceTypeDefinition((GraphQLInterfaceTypeDefinition)node),
            ASTNodeKind.UnionTypeDefinition => PrintUnionTypeDefinition((GraphQLUnionTypeDefinition)node),
            ASTNodeKind.EnumTypeDefinition => PrintEnumTypeDefinition((GraphQLEnumTypeDefinition)node),
            ASTNodeKind.EnumValueDefinition => PrintEnumValueDefinition((GraphQLEnumValueDefinition)node),
            ASTNodeKind.InputObjectTypeDefinition => PrintInputObjectTypeDefinition((GraphQLInputObjectTypeDefinition)node),

            ASTNodeKind.ObjectTypeExtension => PrintTypeExtensionDefinition((GraphQLObjectTypeExtension)node),
            ASTNodeKind.RootOperationTypeDefinition => PrintOperationTypeDefinition((GraphQLRootOperationTypeDefinition)node),

            ASTNodeKind.DirectiveDefinition => PrintDirectiveDefinition((GraphQLDirectiveDefinition)node),
            _ => string.Empty,
        };
    }

    string Block(IEnumerable<string> enumerable)
    {
        if (enumerable == null || !enumerable.Any())
        {
            return null;
        }

        return "{" + Environment.NewLine + Indent(Join(enumerable, Environment.NewLine)) + Environment.NewLine + "}";
    }

    string Indent(string input)
    {
        if (input.IsNot()) return null;

        return "  " + input.Replace(Environment.NewLine, Environment.NewLine + "  ");
    }

    string Join(IEnumerable<string> collection, string separator = "")
    {
        if (collection == null) return "";

        collection = collection?.Where(e => e.Is()); 
        
        try
        {
            if (collection.IsNot()) return "";

            return string.Join(separator ?? "", collection);
        }
        catch
        {
            return null;
        }
    }

    string PrintArgument(GraphQLArgument argument)
    {
        string text = PrintName(argument.Name);
        string text2 = Print(argument.Value);
        return text + ": " + text2;
    }

    string PrintBooleanValue(GraphQLBooleanValue node)
    {
        return node.Value + "";
    }

    string PrintDirective(GraphQLDirective directive)
    {
        string text = PrintName(directive.Name);
        IEnumerable<string> collection = directive.Arguments?.Select(PrintArgument);
        return "@" + text + Wrap("(", Join(collection, ", "), ")");
    }

    string PrintLocation(DirectiveLocation location)
    {
        return "";
    }

    string PrintDirectiveDefinition(GraphQLDirectiveDefinition node)
    {
        string text = PrintName(node.Name);
        IEnumerable<string> enumerable = node.Arguments?.Select(Print);
        IEnumerable<string> collection = node.Locations?.Items.Select(PrintLocation);
        return Join(new string[5]
        {
            "directive @",
            text,
            enumerable.All((string e) => !e.Contains(Environment.NewLine)) ? Wrap("(", Join(enumerable, ", "), ")") : Wrap("(" + Environment.NewLine, Indent(Join(enumerable, Environment.NewLine)), Environment.NewLine + ")"),
            " on ",
            Join(collection, " | ")
        });
    }

    string PrintDocument(GraphQLDocument node)
    {
        IEnumerable<string> collection = node.Definitions?.Select(Print);

        return Join(collection, Environment.NewLine + Environment.NewLine);
    }

    string PrintEnumTypeDefinition(GraphQLEnumTypeDefinition node)
    {
        string text = PrintName(node.Name);
        IEnumerable<string> collection = node.Directives?.Select(PrintDirective);
        IEnumerable<string> enumerable = node.Values?.Select(PrintEnumValueDefinition);
        return Join(new string[4]
        {
            "enum",
            text,
            Join(collection, " "),
            Block(enumerable) ?? "{ }"
        }, " ");
    }

    string PrintEnumValue(GraphQLEnumValue node)
    {
        return node.Name + "";
    }

    string PrintEnumValueDefinition(GraphQLEnumValueDefinition node)
    {
        string text = PrintName(node.Name);
        IEnumerable<string> collection = node.Directives?.Select(PrintDirective);
        return Join(new string[2]
        {
            text,
            Join(collection, " ")
        }, " ");
    }

    string PrintFieldDefinition(GraphQLFieldDefinition node)
    {
        string text = PrintName(node.Name);
        IEnumerable<string> collection = node.Directives?.Select(PrintDirective);
        IEnumerable<string> enumerable = node.Arguments?.Select(PrintInputValueDefinition);
        string text2 = Print(node.Type);
        return Join(new string[5]
        {
            text,
            (enumerable != null && enumerable.All((string e) => !e.Contains(Environment.NewLine))) ? Wrap("(", Join(enumerable, ", "), ")") : Wrap("(" + Environment.NewLine, Indent(Join(enumerable, Environment.NewLine)), Environment.NewLine + "aaa)"),
            ": ",
            text2,
            Wrap(" ", Join(collection, " "))
        });
    }

    private string PrintFieldNode(GraphQLFieldNode node)
    {
        string maybeString = PrintName(node.Alias);
        string text = PrintName(node.Name);
        IEnumerable<string> collection = node.Arguments?.Select(PrintArgument);
        IEnumerable<string> collection2 = node.Directives?.Select(PrintDirective);

        string text2 = PrintSelectionSet(node.SelectionSet);

        return Join(new string[3]
        {
                Wrap(string.Empty, maybeString, ": ") + text + Wrap("(", Join(collection, ", "), ")"),
                Join(collection2, " "),
                text2
        });
    }

    string PrintFloatValue(GraphQLFloatValue node)
    {
        return node.Value + "";
    }

    string PrintFragmentDefinition(GraphQLFragmentDefinition node)
    {
        string text = PrintName(node?.TypeCondition?.Type?.Name);
        string text2 = PrintNamedType(node?.TypeCondition?.Type);
        IEnumerable<string> collection = node.Directives?.Select(PrintDirective);
        string text3 = PrintSelectionSet(node.SelectionSet);
        return "fragment " + text + " on " + text2 + " " + Wrap(string.Empty, Join(collection, " "), " ") + text3;
    }

    string PrintFragmentSpread(GraphQLFragmentSpread node)
    {
        string text = PrintName(node.FragmentName.Name);
        IEnumerable<string> collection = node.Directives?.Select(PrintDirective);
        return "..." + text + Wrap(string.Empty, Join(collection, " "));
    }

    string PrintInlineFragment(GraphQLInlineFragment node)
    {
        string maybeString = PrintNamedType(node.TypeCondition.Type);
        IEnumerable<string> collection = node.Directives?.Select(PrintDirective);
        string text = PrintSelectionSet(node.SelectionSet);
        return Join(new string[4]
        {
            "...",
            Wrap("on ", maybeString),
            Join(collection, " "),
            text
        }, " ");
    }

    string PrintInputObjectTypeDefinition(GraphQLInputObjectTypeDefinition node)
    {
        string text = PrintName(node.Name);
        IEnumerable<string> collection = node.Directives?.Select(PrintDirective);
        IEnumerable<string> enumerable = node.Fields?.Select(PrintInputValueDefinition);
        return Join(new string[4]
        {
            "input",
            text,
            Join(collection, " "),
            Block(enumerable) ?? "{ }"
        }, " ");
    }

    string PrintInputValueDefinition(GraphQLInputValueDefinition node)
    {
        string text = PrintName(node.Name);
        string text2 = Print(node.Type);
        IEnumerable<string> collection = node.Directives?.Select(PrintDirective);
        string maybeString = Print(node.DefaultValue);
        return Join(new string[3]
        {
            text + ": " + text2,
            Wrap("= ", maybeString),
            Join(collection, " ")
        }, " ");
    }

    string PrintInterfaceTypeDefinition(GraphQLInterfaceTypeDefinition node)
    {
        string text = PrintName(node.Name);
        IEnumerable<string> collection = node.Directives?.Select(PrintDirective);
        IEnumerable<string> enumerable = node.Fields?.Select(PrintFieldDefinition);
        return Join(new string[4]
        {
            "interface",
            text,
            Join(collection, " "),
            Block(enumerable) ?? "{ }"
        }, " ");
    }

    string PrintIntValue(GraphQLIntValue node)
    {
        return node.Value + "";
    }

    string PrintListType(GraphQLListType node)
    {
        string text = Print(node.Type);
        return "[" + text + "]";
    }

    string PrintListValue(GraphQLListValue node)
    {
        IEnumerable<string> collection = node.Values?.Select(Print);
        return "[" + Join(collection, ", ") + "]";
    }

    string PrintName(GraphQLName name)
    {
        if (name == (string)null) return "";
        if (name == "") return "";

        return name?.StringValue + "";
    }

    string PrintNamedType(GraphQLNamedType node)
    {
        if (node == null)
        {
            return string.Empty;
        }

        return PrintName(node.Name);
    }

    string PrintNonNullType(GraphQLNonNullType node)
    {
        return Print(node.Type) + "!";
    }

    string PrintNullValue()
    {
        return "null";
    }

    string PrintObjectField(GraphQLObjectField node)
    {
        string text = PrintName(node.Name);
        string text2 = Print(node.Value);
        return text + ": " + text2;
    }

    string PrintObjectTypeDefinition(GraphQLObjectTypeDefinition node)
    {
        string text = PrintName(node.Name);
        IEnumerable<string> collection = node.Interfaces?.Select(PrintNamedType);
        IEnumerable<string> collection2 = node.Directives?.Select(PrintDirective);
        IEnumerable<string> enumerable = node.Fields?.Select(PrintFieldDefinition);
        return Join(new string[5]
        {
            "type",
            text,
            Wrap("implements ", Join(collection, " & ")),
            Join(collection2, " "),
            Block(enumerable) ?? "{ }"
        }, " ");
    }

    string PrintObjectValue(GraphQLObjectValue node)
    {
        IEnumerable<string> collection = node.Fields?.Select(PrintObjectField);

        var val = "{" + Join(collection, ", ") + "}";

        if (val == "{}")
        {
            return "null";
        }

        return val;
    }

    string PrintOperationDefinition(GraphQLOperationDefinition definition)
    {
        string text = PrintName(definition.Name);
        string text2 = Join(definition.Directives?.Select(PrintDirective), " ");
        string text3 = PrintSelectionSet(definition.SelectionSet);
        string text4 = Wrap("(", Join(definition.Variables?.Select(PrintVariableDefinition), ", "), ")");
        string text5 = definition.Operation.ToString().ToLower();
        if (!string.IsNullOrWhiteSpace(text) || !string.IsNullOrWhiteSpace(text) || !string.IsNullOrWhiteSpace(text) || definition.Operation != 0)
        {
            return Join(new string[4]
            {
                text5,
                Join(new string[2] { text, text4 }),
                text2,
                text3
            }, " ");
        }

        return text3;
    }

    string PrintOperationType(OperationType operationType, GraphQLNamedType type)
    {
        string text = operationType.ToString().ToLower();
        string text2 = PrintNamedType(type);
        return text + ": " + text2;
    }

    string PrintOperationTypeDefinition(GraphQLRootOperationTypeDefinition node)
    {
        return node.Operation.ToString() + ": " + PrintNamedType(node.Type);
    }

    string PrintScalarTypeDefinition(GraphQLScalarTypeDefinition node)
    {
        var text = PrintName(node.Name);
        IEnumerable<string> collection = node.Directives?.Select(PrintDirective);
        return Join(new string[3]
        {
            "scalar",
            text,
            Join(collection, " ")
        }, " ");
    }

    string PrintSchemaDefinition(GraphQLSchemaDefinition node)
    {
        IEnumerable<string> collection = node.Directives?.Select(PrintDirective);
        IEnumerable<string> enumerable = node.OperationTypes?.Select(x => PrintOperationType(x.Operation, x.Type));

        return Join(new string[3]
        {
            "schema",
            Join(collection, " "),
            Block(enumerable) ?? "{ }"
        }, " ");
    }

    string PrintSelectionSet(GraphQLSelectionSet selectionSet)
    {
        if (selectionSet == null)
        {
            return string.Empty;
        }

        return Block(selectionSet.Selections?.Select(Print));
    }

    string PrintStringValue(GraphQLStringValue node)
    {
        return "\"" + node.Value + "\"";
    }

    string PrintTypeExtensionDefinition(GraphQLObjectTypeExtension node)
    {
        return "extend " + Print(node);
    }

    string PrintUnionTypeDefinition(GraphQLUnionTypeDefinition node)
    {
        string text = PrintName(node.Name);
        IEnumerable<string> collection = node.Directives?.Select(PrintDirective);
        IEnumerable<string> enumerable = node.Types?.Select(PrintNamedType);
        return Join(new string[4]
        {
            "union",
            text,
            Join(collection, " "),
            (enumerable != null && enumerable.Any()) ? ("= " + Join(enumerable, " | ")) : string.Empty
        }, " ");
    }

    string PrintVariable(GraphQLVariable variable)
    {
        return "$" + variable.Name.Value;
    }

    string PrintVariableDefinition(GraphQLVariableDefinition variableDefinition)
    {
        string text = PrintVariable(variableDefinition.Variable);
        string text2 = Print(variableDefinition.Type);
        string maybeString = variableDefinition.DefaultValue?.ToString();
        return Join(new string[4]
        {
            text,
            ": ",
            text2,
            Wrap(" = ", maybeString)
        });
    }

    string Wrap(string start, string maybeString, string end = "")
    {

        if (maybeString.Is()) return start + maybeString + end;

        return null;
    }
}