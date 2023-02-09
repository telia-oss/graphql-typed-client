using System.Linq.Expressions;

namespace Telia.LinqToGraphQLToModel.Models;

internal class CallChain
{
    List<ChainLink> links;
    Expression node;
    bool isTerminal;

    public IEnumerable<ChainLink> Links => links;
    public Expression Node => node;
    public bool IsTerminal => isTerminal;

    public CallChain(List<ChainLink> links, Expression node, bool isTerminal)
    {
        this.links = links;
        this.node = node;
        this.isTerminal = isTerminal;
    }
}
