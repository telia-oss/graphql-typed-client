using System.Collections;
using System.Reflection;

using Telia.GraphQL.Client.Models;
using Telia.GraphQL.Schema.Attributes;

namespace Telia.GraphQL.Client;

internal class SelectionChainExpander
{
    QueryContext context;

    public SelectionChainExpander(QueryContext context)
    {
        this.context = context;
    }

    public IEnumerable<CallChain> Expand()
    {
        return Expand(context.SelectionChains, new Stack<string>());
    }

    public IEnumerable<CallChain> Expand(IEnumerable<CallChain> selectionChains, Stack<string> visitedTypes)
    {
        var result = new List<CallChain>();

        foreach (var chain in selectionChains)
        {
            if (!chain.IsTerminal) continue;

            result.AddRange(Expand(chain, chain.Node.Type, visitedTypes));
        }

        return result;
    }

    public IEnumerable<CallChain> Expand(CallChain chainPrefix, Type type, Stack<string> visitedTypes)
    {
        if (typeof(IEnumerable).IsAssignableFrom(type) && type.IsGenericType)
        {
            var memberType = type.GetGenericArguments().Single();

            return Expand(chainPrefix, memberType, visitedTypes);
        }

        var result = new List<CallChain>();
        var graphQLTypeAttribute = type.GetCustomAttribute<GraphQLTypeAttribute>();

        if (graphQLTypeAttribute == null) return result;

        visitedTypes.Push(graphQLTypeAttribute.Name);

        foreach (var field in type.GetProperties())
        {
            var fieldAttribute = field.GetCustomAttribute<GraphQLFieldAttribute>();

            if (fieldAttribute == null) continue;

            var copyOfLinks = chainPrefix.Links.ToList();
            copyOfLinks.Add(new ChainLink(fieldAttribute.Name, false));

            var chain = new CallChain(copyOfLinks, null, true);

            var graphQLTypeAttributeOnField = typeof(IEnumerable).IsAssignableFrom(field.PropertyType) && field.PropertyType.IsGenericType
                ? field.PropertyType.GetGenericArguments().Single().GetCustomAttribute<GraphQLTypeAttribute>()
                : field.PropertyType.GetCustomAttribute<GraphQLTypeAttribute>();

            if (!visitedTypes.Contains(graphQLTypeAttributeOnField?.Name))
            {
                if (!field.PropertyType.IsEnum && graphQLTypeAttributeOnField != null)
                {
                    result.AddRange(Expand(chain, field.PropertyType, visitedTypes));
                }
                else
                {
                    result.Add(chain);
                }
            }
        }

        visitedTypes.Pop();

        return result;
    }
}
