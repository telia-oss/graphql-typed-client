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
        private readonly QueryContext _context;

        public SelectionChainConverter(QueryContext context)
        {
            _context = context;
        }

        public GraphQLSelectionSet Convert(IEnumerable<ChainLink> links)
        {
            var index = 0;

            return Convert(links, string.Empty, ref index);
        }

        public GraphQLSelectionSet Convert(IEnumerable<ChainLink> links, string path)
        {
            var index = 0;

            return Convert(links, path, ref index);
        }

        public GraphQLSelectionSet Convert(IEnumerable<ChainLink> links, string path, ref int index)
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
                            Value = GetGraphQLValueFrom(arg.Value)
                        }).ToList(),
                        SelectionSet = this.Convert(link.Children, alias == null ? path : $"{path}.{alias.Value}")
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
                        SelectionSet = this.Convert(link.Children, path, ref index)
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

        private GraphQLValue GetGraphQLValueFrom(object value)
        {
            if (value == null)
            {
                return new GraphQLScalarValue(ASTNodeKind.NullValue);
            }

            if (value is Int32)
            {
                return new GraphQLScalarValue(ASTNodeKind.IntValue)
                {
                    Value = value.ToString()
                };
            }

            if (value is string)
            {
                return new GraphQLScalarValue(ASTNodeKind.StringValue)
                {
                    Value = value as string
                };
            }

            if (value is Single)
            {
                return new GraphQLScalarValue(ASTNodeKind.FloatValue)
                {
                    Value = value.ToString()
                };
            }

            if (value is Boolean)
            {
                return new GraphQLScalarValue(ASTNodeKind.BooleanValue)
                {
                    Value = value.ToString().ToLower()
                };
            }

            if (value is Boolean)
            {
                return new GraphQLScalarValue(ASTNodeKind.BooleanValue)
                {
                    Value = value.ToString().ToLower()
                };
            }

            if (value is DateTime)
            {
                return new GraphQLScalarValue(ASTNodeKind.StringValue)
                {
                    Value = ((DateTime)value).ToString("yyyy-MM-ddTHH:mm:ssZ")
                };
            }

            if (value is InputObjectValue)
            {
                return CreateGraphQLObject((InputObjectValue)value);
            }

            var valueType = value.GetType();

			if (valueType.IsEnum)
			{
				return new GraphQLScalarValue(ASTNodeKind.EnumValue)
				{
					Value = value.ToString()
				};
			}

            if (valueType.IsEnumerable())
            {
                var enumerable = value as IEnumerable;
                var listValues = new List<GraphQLValue>();

                foreach (var member in enumerable)
                {
                    listValues.Add(this.GetGraphQLValueFrom(member));
                }

                return new GraphQLListValue(ASTNodeKind.ListValue)
                {
                    Values = listValues
                };
            }

            throw new NotImplementedException($"Type {value.GetType()} is not implemented");
        }

        private GraphQLValue CreateGraphQLObject(InputObjectValue value)
        {
            var valueType = value.ObjectType;
            var properties = valueType.GetProperties();

            return new GraphQLObjectValue()
            {
                Fields = value.Select(prop => new GraphQLObjectField
                {
                    Name = new GraphQLName
                    {
                        Value = properties.Single(e => e.Name == prop.Key).GetCustomAttribute<GraphQLFieldAttribute>(true).Name
                    },
                    Value = this.GetGraphQLValueFrom(prop.Value)
                }).ToList()
            };
        }
    }
}
