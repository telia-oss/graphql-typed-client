using GraphQLParser.AST;

using Telia.GraphQLPrinter.Models;
using Telia.LinqToGraphQLToModel.Models;

namespace Telia.LinqToGraphQLToModel;

internal class SelectionChainConverter
{
    QueryContext _context;

    public SelectionChainConverter(QueryContext context)
    {
        _context = context;
    }

    public GraphQLSelectionSet Convert(
        IEnumerable<ChainLink> links,
        GraphQLVariablesDefinition variablesDefinition,
        Dictionary<string, object> variableValues)
    {
        var index = 0;

        return Convert(links, variablesDefinition, variableValues, string.Empty, ref index);
    }

    public GraphQLSelectionSet Convert(
        IEnumerable<ChainLink> links,
        GraphQLVariablesDefinition variablesDefinition,
        Dictionary<string, object> variableValues,
        string path)
    {
        var index = 0;

        return Convert(links, variablesDefinition, variableValues, path, ref index);
    }

    public GraphQLSelectionSet Convert(
        IEnumerable<ChainLink> links,
        GraphQLVariablesDefinition variablesDefinition,
        Dictionary<string, object> variableValues,
        string path,
        ref int index)
    {
        if (links == null) return null;

        var selections = new List<ASTNode>();

        foreach (var link in links)
        {
            if (string.IsNullOrWhiteSpace(link.Fragment))
            {
                var alias = link.UseAlias ? $"field{index++}".ToGraphQlName() : null;

                if (link.UseAlias)
                {
                    foreach (var node in link.Nodes)
                    {
                        _context.AddBinding(node, $"{path}.{alias.Value}".Substring(1));
                    }
                }

                selections.Add(new GraphQLFieldNode()
                {
                    Name = link.FieldName.ToGraphQlName(),
                    Alias = alias,
                    Arguments = new GraphQLArguments(link.Arguments?.Select(arg =>
                        new GraphQLArgument(arg.Name.ToGraphQlName(), CreateVariable(arg, variablesDefinition, variableValues))
                    ).ToList() ?? new List<GraphQLArgument>()),
                    SelectionSet = this.Convert(link.Children, variablesDefinition, variableValues, !link.UseAlias ? path : $"{path}.{alias.Value}")
                });
            }
            else
            {
                selections.Add(new GraphQLInlineFragment(null)
                {
                    TypeCondition = new GraphQLTypeCondition(null)
                    {
                        Type = new GraphQLNamedType(null)
                        {
                            Name = link.Fragment.ToGraphQlName()
                        }
                    },
                    SelectionSet = this.Convert(link.Children, variablesDefinition, variableValues, path, ref index)
                });
            }
        }

        if (selections.Any())
        {
            selections.Add(new GraphQLFieldNode()
            {
                Name = "__typename".ToGraphQlName()
            });
        }

        return new GraphQLSelectionSet(selections);
    }

    GraphQLValue CreateVariable(ChainLinkArgument argument, GraphQLVariablesDefinition variablesDefinition, Dictionary<string, object> variableValues)
    {
        if (argument.Value == null)
        {
            return new GraphQLObjectValue();
        }

        var variableDefinitions = variablesDefinition.Items;

        var variableName = $"var_{variableDefinitions.Count}";
        var variable = new GraphQLVariable(null)
        {
            Name = variableName.ToGraphQlName()
        };

        variableDefinitions.Add(new GraphQLVariableDefinition(variable, new GraphQLNamedType(null)
        {
            Name = argument.GraphQLType.ToGraphQlName()
        }));

        variableValues.Add(variableName, argument.Value);

        return variable;
    }
}
