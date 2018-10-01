using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Telia.GraphQL.Client
{
	internal class QueryContext
	{
		private Dictionary<ParameterExpression, List<ChainLink>> parameterToChain;

		internal List<CallChain> SelectionChains { get; private set; }
		internal Dictionary<Expression, string> Bindings { get; private set; }

		public QueryContext()
		{
			this.parameterToChain = new Dictionary<ParameterExpression, List<ChainLink>>();
			this.SelectionChains = new List<CallChain>();
			this.Bindings = new Dictionary<Expression, string>();
		}

		internal void AddParameterToCallChainBinding(ParameterExpression parameterExpression, List<ChainLink> chainPrefix)
		{
			this.parameterToChain.Add(parameterExpression, chainPrefix);
		}

		internal IEnumerable<ChainLink> GetChainPrefixFrom(ParameterExpression parameterExpression)
		{
			if (this.parameterToChain.ContainsKey(parameterExpression))
			{
				return this.parameterToChain[parameterExpression];
			}

			return new ChainLink[] { };
		}
	}
}
