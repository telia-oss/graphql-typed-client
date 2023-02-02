using System.Linq.Expressions;

using Newtonsoft.Json.Linq;

using Telia.GraphQL.Client.Models;

namespace Telia.GraphQL.Client;

internal class QueryContext
{
    Dictionary<ParameterExpression, List<ChainLink>> parameterToChain;
    Dictionary<Expression, string> bindings;
    Dictionary<ParameterExpression, JToken> parameterToModelBindings;
    Dictionary<Expression, object> argumentCache;

    internal List<CallChain> SelectionChains { get; set; }

    public QueryContext()
    {
        this.parameterToChain = new Dictionary<ParameterExpression, List<ChainLink>>();
        this.SelectionChains = new List<CallChain>();
        this.parameterToModelBindings = new Dictionary<ParameterExpression, JToken>();
        this.bindings = new Dictionary<Expression, string>();
        this.argumentCache = new Dictionary<Expression, object>();
    }

    internal object GetValueFromArgumentExpression(string argumentName, Expression argument)
    {
        if (argumentCache.ContainsKey(argument))
        {
            return argumentCache[argument];
        }

        var result = Expression.Lambda(argument).Compile().DynamicInvoke();

        argumentCache.Add(argument, result);

        return result;
    }

    internal void AddParameterToCallChainBinding(ParameterExpression parameterExpression, List<ChainLink> chainPrefix)
    {
        this.parameterToChain.Add(parameterExpression, chainPrefix);
    }

    internal void AddBinding(Expression node, string bindingPath)
    {
        if (node != null)
            this.bindings.Add(node, bindingPath);
    }

    internal IEnumerable<ChainLink> GetChainPrefixFrom(ParameterExpression parameterExpression)
    {
        if (this.parameterToChain.ContainsKey(parameterExpression))
        {
            return this.parameterToChain[parameterExpression];
        }

        return new ChainLink[] { };
    }
}
