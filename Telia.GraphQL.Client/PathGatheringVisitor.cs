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

		private readonly QueryContext context;

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

            this.context.SelectionChains.Add(new CallChain(chain, node));

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
				this.context.SelectionChains.Add(new CallChain(chain, node));
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

			if (current.NodeType == ExpressionType.Parameter)
			{
				chain.AddRange(this.context.GetChainPrefixFrom(current as ParameterExpression));
			}

			return current;
        }

        private Expression AddToChainFromMethods(List<ChainLink> chain, Expression current)
        {
            while (current.NodeType == ExpressionType.Call)
			{
				var methodCallExpression = current as MethodCallExpression;

				if (IsLinqSelectMethod(methodCallExpression))
				{
					var innerLambda = methodCallExpression.Arguments[1] as LambdaExpression;
					var chainPrefix = GetChainToSelectMethod(methodCallExpression);

					this.context.SelectionChains.Add(new CallChain(
						chainPrefix.ToArray().Reverse().ToList(),
						methodCallExpression.Arguments[0]));

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
					this.GetArgumentsFromMethod(methodCallExpression)));

				current = methodCallExpression.Object;
			}

			if (current.NodeType == ExpressionType.MemberAccess)
            {
                return AddToChainFromMembers(chain, current);
            }

            return current;
        }

		private List<ChainLink> GetChainToSelectMethod(MethodCallExpression methodCallExpression)
		{
			var chainPrefix = new List<ChainLink>();

			this.AddToChainFromMethods(chainPrefix, methodCallExpression.Arguments[0]);

			return chainPrefix;
		}

		private static bool IsLinqSelectMethod(MethodCallExpression methodCallExpression)
		{
			return methodCallExpression.Method.IsGenericMethod &&
								SelectMethods.Contains(methodCallExpression.Method.GetGenericMethodDefinition());
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
