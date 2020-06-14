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

        private Expression AddToChainFromExpression(List<ChainLink> chain, Expression current)
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
                var unaryExpression = (UnaryExpression) current;

                chain.Add(new ChainLink(null, false)
                {
                    Fragment = unaryExpression.Type.Name
                });

                return AddToChainFromExpression(chain, unaryExpression.Operand);
            }

            return current;
        }

        private Expression AddToChainFromMembers(List<ChainLink> chain, Expression current)
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

        private Expression AddToChainFromMethods(List<ChainLink> chain, Expression current)
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
					this.GetArgumentsFromMethod(methodCallExpression)));

				current = methodCallExpression.Object;
			}

            return AddToChainFromExpression(chain, current);
        }

		private List<ChainLink> GetChainToSelectMethod(MethodCallExpression methodCallExpression)
		{
			var chainPrefix = new List<ChainLink>();

			this.AddToChainFromMethods(chainPrefix, methodCallExpression.Arguments[0]);

			return chainPrefix;
		}

		private IEnumerable<ChainLinkArgument> GetArgumentsFromMethod(MethodCallExpression methodCallExpression)
        {
            var methodParameters = methodCallExpression.Method.GetParameters();
            var arguments = methodCallExpression.Arguments;

            for (var i = 0; i < arguments.Count; i++)
            {
                var argument = arguments.ElementAt(i);
                var parameter = methodParameters.ElementAt(i);

                var nameAttribute = parameter.GetCustomAttribute<GraphQLArgumentAttribute>();

                var name = nameAttribute != null
                    ? nameAttribute.Name
                    : parameter.Name;

                yield return new ChainLinkArgument()
                {
                    Name = name,
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
                case ExpressionType.MemberInit: return this.GetValueFromMemberInit((MemberInitExpression)argument);
                case ExpressionType.NewArrayInit: return this.GetValueFromNewArrayInit((NewArrayExpression)argument);
            }

            if (argument is UnaryExpression)
            {
                return GetValueFromUnaryExpression((UnaryExpression)argument);
            }

            throw new NotImplementedException($"GetValueFromExpression: unknown NodeType: {argument.NodeType}");
        }

        private object GetValueFromUnaryExpression(UnaryExpression argument)
        {
            var value = this.GetValueFromExpression(argument.Operand);

            if (argument.Method != null)
            {
                return argument.Method.Invoke(value, new object[] { });
            }

            return value;
        }

        private object GetValueFromNewArrayInit(NewArrayExpression argument)
        {
            var array = Activator.CreateInstance(argument.Type, argument.Expressions.Count) as Array;

            for (var i = 0; i < argument.Expressions.Count; i++)
            {
                var value = this.GetValueFromExpression(argument.Expressions[i]);

                array.SetValue(value, i);
            }

            return array;
        }

        private InputObjectValue GetValueFromMemberInit(MemberInitExpression argument)
        {
            var obj = new InputObjectValue(argument.Type);
            var requiredPropertiesToSendIn = argument.Type.GetProperties()
                .Where(e => e.PropertyType.IsValueType && Nullable.GetUnderlyingType(e.PropertyType) == null)
                .ToList();

            foreach (var binding in argument.Bindings)
            {
                switch (binding.BindingType)
                {
                    case MemberBindingType.Assignment:
                        {
                            var assignmentBinding = (MemberAssignment)binding;
                            var value = this.GetValueFromExpression(assignmentBinding.Expression);

                            obj.Add(binding.Member.Name, value);
                            requiredPropertiesToSendIn.RemoveAll(e => e.Name == binding.Member.Name);
                        }
                        break;
                    default: throw new NotImplementedException($"GetValueFromMemberInit: Unknown BindingType: {binding.BindingType}");
                }
            }

            foreach (var prop in requiredPropertiesToSendIn)
            {
                obj.Add(prop.Name, Activator.CreateInstance(prop.PropertyType));
            }

            return obj;
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
                throw new NotImplementedException(
                    $"GetValueFromMemberAccessExpression: Not implemented scenario where constant.NodeType = {constant.NodeType}");
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
