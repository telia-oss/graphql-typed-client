using System.Linq.Expressions;
using System.Reflection;

using Telia.GraphQL.Client.Models;
using Telia.GraphQL.Schema.Attributes;
using Telia.LinqToGraphQL;

namespace Telia.GraphQL.Client;

internal class PathGatheringVisitor : ExpressionVisitor
{
    QueryContext context;

    public PathGatheringVisitor(QueryContext context)
    {
        this.context = context;
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        var chain = new List<ChainLink>();
        var current = node as Expression;

        current = this.AddToChainFromMembers(chain, current);

        chain.Reverse();

        this.context.SelectionChains.Add(new CallChain(chain, node, true));

        return node;
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        var chain = new CallChain(this.context.GetChainPrefixFrom(node).ToList(), node, true);

        if (chain.Links.Any())
            this.context.SelectionChains.Add(chain);

        return base.VisitParameter(node);
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        var chain = new List<ChainLink>();
        var current = node as Expression;

        current = this.AddToChainFromMethods(chain, current);

        chain.Reverse();

        if (chain.Count > 0)
            this.context.SelectionChains.Add(new CallChain(chain, node, true));

        return node;
    }

    Expression AddToChainFromExpression(List<ChainLink> chain, Expression current)
    {
        if (current.NodeType == ExpressionType.MemberAccess)
        {
            return AddToChainFromMembers(chain, current);
        }

        if (current.NodeType == ExpressionType.Call)
        {
            return AddToChainFromMethods(chain, current);
        }

        if (current.NodeType == ExpressionType.Parameter)
        {
            chain.AddRange(this.context.GetChainPrefixFrom(current as ParameterExpression));
        }

        if (current.NodeType == ExpressionType.TypeAs && chain.Count > 0)
        {
            var unaryExpression = (UnaryExpression)current;

            chain.Add(new ChainLink(null, false)
            {
                Fragment = unaryExpression.Type.Name
            });

            return AddToChainFromExpression(chain, unaryExpression.Operand);
        }

        return current;
    }

    Expression AddToChainFromMembers(List<ChainLink> chain, Expression current)
    {
        while (current.NodeType == ExpressionType.MemberAccess)
        {
            var memberExpression = current as MemberExpression;

            var attribute = memberExpression.Member.GetCustomAttribute<GraphQLFieldAttribute>();

            if (attribute == null)
            {
                return base.VisitMember(memberExpression);
            }

            chain.Add(new ChainLink(attribute.Name, true));

            current = memberExpression.Expression;
        }

        return AddToChainFromExpression(chain, current);
    }

    Expression AddToChainFromMethods(List<ChainLink> chain, Expression current)
    {
        while (current.NodeType == ExpressionType.Call)
        {
            var methodCallExpression = current as MethodCallExpression;

            if (Utils.IsLinqSelectMethod(methodCallExpression))
            {
                var innerLambda = methodCallExpression.Arguments[1] as LambdaExpression;
                var chainPrefix = GetChainToSelectMethod(methodCallExpression);

                this.context.SelectionChains.Add(new CallChain(
                    chainPrefix.ToArray().Reverse().ToList(),
                    methodCallExpression.Arguments[0],
                false));

                this.context.AddParameterToCallChainBinding(innerLambda.Parameters.First(), chainPrefix);

                var childVisitor = new PathGatheringVisitor(this.context);
                childVisitor.Visit(innerLambda.Body);

                return methodCallExpression.Arguments[0];
            }

            var attribute = methodCallExpression.Method.GetCustomAttribute<GraphQLFieldAttribute>();

            if (attribute == null)
            {
                return base.VisitMethodCall(methodCallExpression);
            }

            chain.Add(new ChainLink(
                attribute.Name,
            true,
                this.GetArgumentsFromMethod(methodCallExpression, context)));

            current = methodCallExpression.Object;
        }

        return AddToChainFromExpression(chain, current);
    }

    List<ChainLink> GetChainToSelectMethod(MethodCallExpression methodCallExpression)
    {
        var chainPrefix = new List<ChainLink>();

        this.AddToChainFromMethods(chainPrefix, methodCallExpression.Arguments[0]);

        return chainPrefix;
    }

    IEnumerable<ChainLinkArgument> GetArgumentsFromMethod(MethodCallExpression methodCallExpression, QueryContext context)
    {
        var methodParameters = methodCallExpression.Method.GetParameters();
        var arguments = methodCallExpression.Arguments;

        for (var i = 0; i < arguments.Count; i++)
        {
            var argument = arguments.ElementAt(i);
            var parameter = methodParameters.ElementAt(i);

            var nameAttribute = parameter.GetCustomAttribute<GraphQLArgumentAttribute>();

            if (nameAttribute == null)
            {
                throw new InvalidOperationException($"Parameter {parameter.Name} doesn't have GraphQLArgument attribute");
            }

            yield return new ChainLinkArgument()
            {
                Name = nameAttribute.Name,
                Value = context.GetValueFromArgumentExpression(parameter.Name, argument),
                GraphQLType = nameAttribute.GraphQLType
            };
        }
    }
}
