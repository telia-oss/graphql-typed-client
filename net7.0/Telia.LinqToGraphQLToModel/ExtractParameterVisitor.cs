using System.Linq.Expressions;

namespace Telia.GraphQL.Client;

public class ExtractParameterVisitor : ExpressionVisitor
{
	public List<ParameterExpression> UsedParameters;

	public ExtractParameterVisitor()
	{
		this.UsedParameters = new List<ParameterExpression>();
	}


	protected override Expression VisitParameter(ParameterExpression node)
	{
		this.UsedParameters.Add(node);

		return base.VisitParameter(node);
	}
}
