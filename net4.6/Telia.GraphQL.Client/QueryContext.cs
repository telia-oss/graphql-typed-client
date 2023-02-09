﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Telia.GraphQL.Client
{
	internal class QueryContext
	{
		private Dictionary<ParameterExpression, List<ChainLink>> parameterToChain;
		private Dictionary<Expression, string> bindings;
		private Dictionary<ParameterExpression, JToken> parameterToModelBindings;
        private Dictionary<Expression, object> argumentCache; 

		internal List<CallChain> SelectionChains { get; private set; }

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

            try
            {
                var result = Expression.Lambda(argument).Compile().DynamicInvoke();

                argumentCache.Add(argument, result);

                return result;
            }
            catch (TargetInvocationException ex)
            {
                throw new ArgumentEvaluationException(argumentName, ex.InnerException);
            }
            catch (Exception ex)
            {
                throw new ArgumentEvaluationException(argumentName, ex);
            }
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
			return this.bindings.ContainsKey(node);
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

		private ParameterExpression GetParameterFrom(Expression node)
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
}
