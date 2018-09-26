using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GraphQLTypedClient.Client
{
    internal class ResponseComposer
    {
        private static MethodInfo[] SelectMethods = typeof(Enumerable)
            .GetMethods()
            .Where(e => e.Name == "Select")
            .ToArray();

        public TResponse Compose<TQueryType, TResponse>(
            Expression<Func<TQueryType, TResponse>> selector,
            JObject response,
            IDictionary<Expression, string> bindings)
        {
            var visitor = new ResponseComposerVisitor(response, bindings);

            var substituted = visitor.Visit(selector) as Expression<Func<TQueryType, TResponse>>;

            return substituted.Compile()(default(TQueryType));
        }

        private class ResponseComposerVisitor : ExpressionVisitor
        {
            private JToken response;
            private IDictionary<Expression, string> bindings;
            private string bindingPrefix;

            public ResponseComposerVisitor(
                JToken response,
                IDictionary<Expression, string> bindings,
                string bindingPrefix = "")
            {
                this.response = response;
                this.bindings = bindings;
                this.bindingPrefix = bindingPrefix;
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node.Method.IsGenericMethod &&
                    SelectMethods.Contains(node.Method.GetGenericMethodDefinition()))
                {
                    var binding = this.bindings[node.Arguments[0]];
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

                var childVisitor = new ResponseComposerVisitor(element as JToken, this.bindings, bindingPrefix);
                var visited = childVisitor.Visit(childExpression);
                
                return ((LambdaExpression)visited).Body;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return base.VisitParameter(node);
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                if (this.bindings.ContainsKey(node))
                {
                    return Expression.Convert(
                        Expression.Constant(this.GetValueFrom(this.bindings[node], node.Type)), node.Type);
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
