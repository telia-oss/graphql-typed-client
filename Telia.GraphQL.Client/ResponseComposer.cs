using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Telia.GraphQL.Client
{
    internal class ResponseComposer<TQueryType, TReturn>

	{
        private static MethodInfo[] SelectMethods = typeof(Enumerable)
            .GetMethods()
            .Where(e => e.Name == "Select")
            .ToArray();

		private Expression<Func<TQueryType, TReturn>> selector;
		private QueryContext context;

		public ResponseComposer(Expression<Func<TQueryType, TReturn>> selector, QueryContext context)
		{
			this.selector = selector;
			this.context = context;
		}

		public TReturn Compose(JObject response)
        {
            var visitor = new ResponseComposerVisitor(response, this.context);

            var substituted = visitor.Visit(selector) as Expression<Func<TQueryType, TReturn>>;

            return substituted.Compile()(default(TQueryType));
        }

        private class ResponseComposerVisitor : ExpressionVisitor
        {
            private JToken response;
			private QueryContext context;
			private string bindingPrefix;

            public ResponseComposerVisitor(
                JToken response,
				QueryContext context,
                string bindingPrefix = "")
            {
                this.response = response;
                this.context = context;
                this.bindingPrefix = bindingPrefix;
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
				if (node.Method.IsGenericMethod &&
					SelectMethods.Contains(node.Method.GetGenericMethodDefinition()))
				{
					var binding = this.context.Bindings[node.Arguments[0]];
					var model = this.GetValueFrom(binding, typeof(IEnumerable<object>)) as IEnumerable<object>;
					var elementType = node.Type.GetGenericArguments()[0];

					if (model == null)
					{
						return Expression.Constant(null, node.Type);
					}

					var initializers = model
						.Select(e => CreateInitializer(elementType, e, node.Arguments[1], $"{binding}"));

					var arrayExpression = Expression.NewArrayInit(
						elementType,
						initializers);

					return arrayExpression;
				}

				return base.VisitMethodCall(node);
            }

            private Expression CreateInitializer(
                Type elementType,
                object element,
                Expression childExpression,
                string bindingPrefix)
            {
                var ctor = elementType.GetConstructor(new Type[] { });
                var args = new Expression[] { };

				var childVisitor = new ResponseComposerVisitor(element as JToken, this.context, bindingPrefix);
				var visited = childVisitor.Visit(childExpression);

				return ((LambdaExpression)visited).Body;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return base.VisitParameter(node);
            }

            protected override Expression VisitMember(MemberExpression node)
            {
				if (this.context.Bindings.ContainsKey(node))
				{
					return Expression.Convert(
						Expression.Constant(this.GetValueFrom(this.context.Bindings[node], node.Type)), node.Type);
				}

				return node;
            }

            private object GetValueFrom(string binding, Type returnType)
            {
                var token = response.SelectToken(binding.Substring(bindingPrefix.Length));

                return GetValueFromToken(returnType, token);
            }

            private object GetValueFromToken(Type returnType, JToken token)
            {
                if (token == null)
                {
                    return this.GetDefaultValue(returnType);
                }

                if (token.Type == JTokenType.Array)
                {
                    return GetValueFromArray(returnType, (JArray)token);
                }

                if (token.Type == JTokenType.Property)
                {
                    return this.GetValueFromProperty(returnType, (JProperty)token);
                }

                if (token.Type == JTokenType.Object)
                {
                    return this.GetValueFromJObject(returnType, (JObject)token);
                }

                return GetValueFromJValue(returnType, (JValue)token);
            }

            private object GetValueFromJObject(Type returnType, JObject token)
            {
                return token.ToObject(returnType);
            }

            private object GetValueFromProperty(Type returnType, JProperty token)
            {
                return this.GetValueFromToken(returnType, new JObject(token));
            }

            private object GetValueFromArray(Type returnType, JArray array)
            {
                var memberType = returnType.GetGenericArguments()[0];

                var listType = typeof(List<>).MakeGenericType(memberType);
                var addMethod = listType.GetMethod("Add");
                var list = Activator.CreateInstance(listType);

                foreach (var member in array)
                {
                    var value = GetValueFromToken(memberType, member);
                    addMethod.Invoke(list, new object[] { value });
                }

                return list;
            }

            private object GetValueFromJValue(Type returnType, JValue token)
            {
                var value = token?.Value;

                if (value == null)
                {
                    return this.GetDefaultValue(returnType);
                }

                return Convert.ChangeType(value, returnType);
            }

            private object GetDefaultValue(Type t)
            {
                if (t.IsValueType)
                    return Activator.CreateInstance(t);

                return null;
            }
        }
    }
}
