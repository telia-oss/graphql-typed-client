using System.Linq.Expressions;

using Newtonsoft.Json.Linq;

using Telia.LinqToGraphQLToModel.Models;

namespace Telia.LinqToGraphQLToModel;

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

    internal string GetBindingPath(Expression node)
    {
        var parameter = this.GetParameterFrom(node);
        var binding = this.bindings[node];

        if (string.IsNullOrWhiteSpace(binding))
        {
            return null;
        }

        var chainToParameter = this.GetChainPrefixFrom(parameter);
        var parts = binding.Split('.');

        return string.Join(".", parts.Skip(chainToParameter.Count()));
    }

    internal bool ContainsBinding(Expression node)
    {
        if (node == null) return false;

        return this.bindings?.ContainsKey(node) == true;
    }

    internal void AddModelToParameterBinding(ParameterExpression parameterExpression, JToken response)
    {
        this.parameterToModelBindings.Add(parameterExpression, response);
    }

    internal void RemoveModelToParameterBinding(ParameterExpression parameterExpression)
    {
        this.parameterToModelBindings.Remove(parameterExpression);
    }

    internal JToken GetModelFor(Expression node)
    {
        var param = this.GetParameterFrom(node);

        return this.parameterToModelBindings[param];
    }

    ParameterExpression GetParameterFrom(Expression node)
    {
        var visitor = new ExtractParameterVisitor();

        visitor.Visit(node);

        if (visitor.UsedParameters.Count > 1)
        {
            throw new NotImplementedException(
                $"QueryContext::GetParameterFrom: Unsupported scenario where visitor.UsedParameters.Count > 1 ({visitor.UsedParameters})");
        }

        return visitor.UsedParameters.FirstOrDefault();
    }
}
