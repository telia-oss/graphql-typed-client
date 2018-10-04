using GraphQLParser.AST;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Telia.GraphQL.Client
{
    internal class SelectionChainConverter
    {
        public GraphQLSelectionSet Convert(IEnumerable<ChainLink> links)
        {
            if (links == null) return null;

            return new GraphQLSelectionSet()
            {
                Selections = links.Select((e, index) => new GraphQLFieldSelection()
                {
                    Name = new GraphQLName
                    {
                        Value = e.FieldName
                    },
                    Alias = new GraphQLName()
                    {
                        Value = $"field{index}"
                    },
                    Arguments = e.Arguments?.Select(arg => new GraphQLArgument()
                    {
                        Name = new GraphQLName()
                        {
                            Value = arg.Name
                        },
                        Value = GetGraphQLValueFrom(arg.Value)
                    }),
                    SelectionSet = this.Convert(e.Children)
                })
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
    }
}
