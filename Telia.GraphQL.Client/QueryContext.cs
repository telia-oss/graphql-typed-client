using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;

namespace Telia.GraphQL.Client
{
	internal class QueryContext
	{
		private Dictionary<ParameterExpression, List<ChainLink>> parameterToChain;
		private Dictionary<Expression, string> bindings;
		private Dictionary<ParameterExpression, JToken> parameterToModelBindings;

		internal List<CallChain> SelectionChains { get; private set; }

		public QueryContext()
		{
			this.parameterToChain = new Dictionary<ParameterExpression, List<ChainLink>>();
			this.SelectionChains = new List<CallChain>();
			this.parameterToModelBindings = new Dictionary<ParameterExpression, JToken>();
			this.bindings = new Dictionary<Expression, string>();
		}

		internal void AddParameterToCallChainBinding(ParameterExpression parameterExpression, List<ChainLink> chainPrefix)
		{
			this.parameterToChain.Add(parameterExpression, chainPrefix);
		}

		internal void AddBinding(Expression node, string bindingPath)
		{
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
				throw new NotImplementedException();
			}

			return visitor.UsedParameters.FirstOrDefault();
		}
	}
}
