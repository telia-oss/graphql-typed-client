using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Telia.GraphQL.Client
{
	internal class ResponseComposer<TQueryType, TReturn>
	{
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

            return substituted.Compile()(default);
        }

        private class ResponseComposerVisitor : ExpressionVisitor
        {
            private JToken response;
			private QueryContext context;
			private ParameterExpression lambdaParameter;

			public ResponseComposerVisitor(
                JToken response,
				QueryContext context)
            {
                this.response = response;
                this.context = context;
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
				if (Utils.IsLinqSelectMethod(node))
				{
					var binding = this.context.GetBindingPath(node.Arguments[0]);
					var argumentModel = this.context.GetModelFor(node.Arguments[0]);

					var model = this.GetValueFrom(argumentModel, binding, typeof(IEnumerable<object>)) as IEnumerable<object>;
					var elementType = node.Type.GetGenericArguments()[0];

					if (model == null)
					{
						return Expression.Constant(null, node.Type);
					}

					var initializers = model
						.Select(e => CreateInitializer(elementType, e, node.Arguments[1]));

					var arrayExpression = Expression.NewArrayInit(
						elementType,
						initializers);

					return arrayExpression;
				}

                if (this.context.ContainsBinding(node))
                {
                    var model = this.context.GetModelFor(node);

                    return Expression.Convert(
                        Expression.Constant(
                            this.GetValueFrom(model, this.context.GetBindingPath(node), node.Type)), node.Type);
                }

                return base.VisitMethodCall(node);
            }

            private Expression CreateInitializer(
                Type elementType,
                object element,
                Expression childExpression)
            {
                var ctor = elementType.GetConstructor(new Type[] { });
                var args = new Expression[] { };

				var childVisitor = new ResponseComposerVisitor(element as JToken, this.context);
				var visited = childVisitor.Visit(childExpression);

				return ((LambdaExpression)visited).Body;
            }

			protected override Expression VisitLambda<T>(Expression<T> node)
			{
				if (node.Parameters.Count == 1)
				{
					this.lambdaParameter = node.Parameters.First();
					this.context.AddModelToParameterBinding(this.lambdaParameter, this.response);
				}

				var lambda = base.VisitLambda(node);

				if (node.Parameters.Count == 1)
				{
					this.context.RemoveModelToParameterBinding(node.Parameters.First());
				}

				return lambda;
			}

			protected override Expression VisitMember(MemberExpression node)
            {
				if (this.context.ContainsBinding(node))
				{
					var model = this.context.GetModelFor(node);

					return Expression.Convert(
						Expression.Constant(
							this.GetValueFrom(model, this.context.GetBindingPath(node), node.Type)), node.Type);
				}

				return base.VisitMember(node);
            }

            private object GetValueFrom(JToken model, string binding, Type returnType)
            {
                var token = model.SelectToken(binding);

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
				if (!returnType.IsObject())
				{
					return this.GetDefaultValue(returnType);
				}

                return token.ToObject(returnType);
            }

            private object GetValueFromProperty(Type returnType, JProperty token)
            {
                return this.GetValueFromToken(returnType, new JObject(token));
            }

            private object GetValueFromArray(Type returnType, JArray array)
            {
				if (!returnType.IsEnumerable())
				{
					return this.GetDefaultValue(returnType);
				}

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

				if (returnType.IsEnum && value is string enumString)
				{
					return Enum.Parse(returnType, enumString);
				}

                if (returnType == typeof(TimeSpan) && value is string)
                {
                    var date = (DateTime)Convert.ChangeType(value, typeof(DateTime));
                    return date.ToUniversalTime() - DateTime.UtcNow.Date;
                }

                if (returnType == typeof(DateTime) && value is string)
                {
                    try
                    {
                        return ((DateTime) Convert.ChangeType(value, typeof(DateTime))).ToUniversalTime();
                    }
                    catch
                    {
                        return this.GetDefaultValue(returnType);
                    }
                }

                if (value.GetType() == Nullable.GetUnderlyingType(returnType))
                {
                    return value;
                }
                else if (Nullable.GetUnderlyingType(returnType) != null)
                {
                    return GetValueFromJValue(Nullable.GetUnderlyingType(returnType), token);
                }

                try
				{
					return Convert.ChangeType(value, returnType);
				}
				catch
				{
					return this.GetDefaultValue(returnType);
				}
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
