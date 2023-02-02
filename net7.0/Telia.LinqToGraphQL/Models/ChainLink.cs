using System.Linq.Expressions;

namespace Telia.GraphQL.Client.Models;

internal class ChainLink
{
    public string FieldName { get; }
    public bool UseAlias { get; }
    public string Fragment { get; set; }
    public IEnumerable<ChainLinkArgument> Arguments { get; }
    public IEnumerable<ChainLink> Children { get; set; }
    public List<Expression> Nodes { get; internal set; }

    public ChainLink(string fieldName, bool useAlias, IEnumerable<ChainLinkArgument> arguments = null)
    {
        FieldName = fieldName;
        Arguments = arguments;
        UseAlias = useAlias;
        Nodes = new List<Expression>();
    }

    public ChainLink(
        string fieldName,
        bool useAlias,
        IEnumerable<ChainLinkArgument> arguments,
        IEnumerable<ChainLink> children) : this(fieldName, useAlias, arguments)
    {
        Children = children;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is ChainLink))
        {
            return false;
        }

        var link = obj as ChainLink;

        if (link.FieldName != FieldName)
        {
            return false;
        }

        if (link.Arguments?.Count() != Arguments?.Count())
        {
            return false;
        }

        if (link.Fragment != Fragment)
        {
            return false;
        }

        for (var i = 0; i < Arguments?.Count(); i++)
        {
            if (!link.Arguments.ElementAt(i).Equals(Arguments.ElementAt(i)))
            {
                return false;
            }
        }

        return true;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
