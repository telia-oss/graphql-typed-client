using GraphQLParser.AST;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Telia.GraphQL.Client.Attributes;

namespace Telia.GraphQL.Client
{
    internal class SelectionChainConverter
    {
        QueryContext _context;

        public SelectionChainConverter(QueryContext context)
        {
            _context = context;
        }

        public GraphQLSelectionSet Convert(
            IEnumerable<ChainLink> links,
            List<GraphQLVariableDefinition> variableDefinitions,
            Dictionary<string, object> variableValues)
        {
            var index = 0;

            return Convert(links, variableDefinitions, variableValues, string.Empty, ref index);
        }

        public GraphQLSelectionSet Convert(
            IEnumerable<ChainLink> links,
            List<GraphQLVariableDefinition> variableDefinitions,
            Dictionary<string, object> variableValues,
            string path)
        {
            var index = 0;

            return Convert(links, variableDefinitions, variableValues, path, ref index);
        }

        public GraphQLSelectionSet Convert(
            IEnumerable<ChainLink> links,
            List<GraphQLVariableDefinition> variableDefinitions,
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
                    var alias = link.UseAlias
                        ? new GraphQLName {Value = $"field{index++}"}
                        : null;

                    if (alias != null)
                    {
                        foreach (var node in link.Nodes)
                        {
                            _context.AddBinding(node, $"{path}.{alias.Value}".Substring(1));
                        }
                    }

                    selections.Add(new GraphQLFieldSelection()
                    {
                        Name = new GraphQLName
                        {
                            Value = link.FieldName
                        },
                        Alias = alias,
                        Arguments = link.Arguments?.Select(arg => new GraphQLArgument()
                        {
                            Name = new GraphQLName()
                            {
                                Value = arg.Name
                            },
                            Value = CreateVariable(arg, variableDefinitions, variableValues)
                        }).ToList(),
                        SelectionSet = this.Convert(link.Children, variableDefinitions, variableValues, alias == null ? path : $"{path}.{alias.Value}")
                    });
                }
                else
                {
                    selections.Add(new GraphQLInlineFragment
                    {
                        TypeCondition = new GraphQLNamedType()
                        {
                            Name = new GraphQLName()
                            {
                                Value = link.Fragment
                            }
                        },
                        SelectionSet = this.Convert(link.Children, variableDefinitions, variableValues, path, ref index)
                    });
                }
            }

            if (selections.Any())
            {
                selections.Add(new GraphQLFieldSelection()
                {
                    Name = new GraphQLName { Value = "__typename" },
                });
            }

            return new GraphQLSelectionSet()
            {
                Selections = selections
            };
        }

        GraphQLValue CreateVariable(ChainLinkArgument argument, List<GraphQLVariableDefinition> variableDefinitions, Dictionary<string, object> variableValues)
        {
            if (argument.Value == null)
            {
                return new GraphQLScalarValue(ASTNodeKind.NullValue);
            }

            var variableName = $"var_{variableDefinitions.Count}";
            var variable = new GraphQLVariable()
            {
                Name = new GraphQLName()
                {
                    Value = variableName
                }
            };

            variableDefinitions.Add(new GraphQLVariableDefinition()
            {
                Variable = variable,
                Type = new GraphQLNamedType()
                {
                    Name = new GraphQLName()
                    {
                        Value = argument.GraphQLType
                    }
                }
            });

            variableValues.Add(variableName, argument.Value);

            return variable;
        }
    }
}
