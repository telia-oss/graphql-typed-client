using Telia.GraphQL.Client.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Telia.GraphQL.Client
{
    internal class PathGatheringVisitor : ExpressionVisitor
    {
        private static MethodInfo[] SelectMethods = typeof(Enumerable)
            .GetMethods()
            .Where(e => e.Name == "Select")
            .ToArray();

        private List<CallChain> selectionChains;

        public PathGatheringVisitor()
        {
            this.selectionChains = new List<CallChain>();
        }

        public IEnumerable<CallChain> GetChains()
        {
            return this.selectionChains;
        }

        protected override Expression VisitNew(NewExpression node)
        {
            return base.VisitNew(node);
        }

        public override Expression Visit(Expression node)
        {
            return base.Visit(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var chain = new List<ChainLink>();
            var current = node as Expression;

            current = this.AddToChainFromMembers(chain, current);

            chain.Reverse();

            this.selectionChains.Add(new CallChain(chain, node));

            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var chain = new List<ChainLink>();
            var current = node as Expression;

            current = this.AddToChainFromMethods(chain, current);

            chain.Reverse();

            if (chain.Any())
            {
                this.selectionChains.Add(new CallChain(chain, node));
            }

            return node;
        }

        private Expression AddToChainFromMembers(List<ChainLink> chain, Expression current)
        {
            while (current.NodeType == ExpressionType.MemberAccess)
            {
                var memberExpression = current as MemberExpression;

                var attribute = memberExpression.Member.GetCustomAttribute<GraphQLFieldAttribute>();

                chain.Add(new ChainLink(attribute.Name));

                current = memberExpression.Expression;
            }

            if (current.NodeType == ExpressionType.Call)
            {
                return AddToChainFromMethods(chain, current);
            }

            return current;
        }

        private Expression AddToChainFromMethods(List<ChainLink> chain, Expression current)
        {
            while (current.NodeType == ExpressionType.Call)
            {
                var methodCallExpression = current as MethodCallExpression;

                if (methodCallExpression.Method.IsGenericMethod &&
                    SelectMethods.Contains(methodCallExpression.Method.GetGenericMethodDefinition()))
                {
                    var chainPrefix = new List<ChainLink>();
                    this.AddToChainFromMethods(chainPrefix, methodCallExpression.Arguments[0]);

                    chainPrefix.Reverse();

                    this.selectionChains.Add(new CallChain(chainPrefix, methodCallExpression.Arguments[0]));

                    var childVisitor = new PathGatheringVisitor();
                    childVisitor.Visit(methodCallExpression.Arguments[1]);

                    var chains = childVisitor.GetChains();

                    foreach (var childChain in chains)
                    {
                        var tmp = chainPrefix.ToList();
                        tmp.AddRange(childChain.Links);

                        this.selectionChains.Add(new CallChain(tmp, childChain.Node));
                    }

                    current = methodCallExpression.Arguments[0];

                    return current;
                }

                var attribute = methodCallExpression.Method.GetCustomAttribute<GraphQLFieldAttribute>();

                if (attribute == null)
                {
                    return base.VisitMethodCall(methodCallExpression);
                }

                chain.Add(new ChainLink(
                    attribute.Name,
                    this.GetArgumentsFromMethod(methodCallExpression)));

                current = methodCallExpression.Object;
            }

            if (current.NodeType == ExpressionType.MemberAccess)
            {
                return AddToChainFromMembers(chain, current);
            }

            return current;
        }

        private IEnumerable<ChainLinkArgument> GetArgumentsFromMethod(MethodCallExpression methodCallExpression)
        {
            var methodParameters = methodCallExpression.Method.GetParameters();
            var arguments = methodCallExpression.Arguments;

            for (var i = 0; i < arguments.Count; i++)
            {
                var argument = arguments.ElementAt(i);

                yield return new ChainLinkArgument()
                {
                    Name = methodParameters.ElementAt(i).Name,
                    Value = this.GetValueFromExpression(argument)
                };
            }
        }

        private object GetValueFromExpression(Expression argument)
        {
            switch (argument.NodeType)
            {
                case ExpressionType.Constant: return ((ConstantExpression)argument).Value;
                case ExpressionType.MemberAccess: return this.GetValueFromMemberAccessExpression((MemberExpression)argument);
            }

            throw new NotImplementedException();
        }

        private object GetValueFromMemberAccessExpression(MemberExpression argument)
        {
            var constant = argument as Expression;
            var listOfMemberAccess = new List<MemberExpression>();

            while (constant.NodeType == ExpressionType.MemberAccess)
            {
                var member = ((MemberExpression)constant);

                listOfMemberAccess.Add(member);
                constant = member.Expression;
            }

            if (constant.NodeType != ExpressionType.Constant)
            {
                throw new NotImplementedException();
            }

            listOfMemberAccess.Reverse();

            var returnValue = ((ConstantExpression)constant).Value;
            
            foreach (var memberAccess in listOfMemberAccess)
            {
                returnValue = memberAccess.Member.GetValue(returnValue);
            }

            return returnValue;
        }
    }
}
