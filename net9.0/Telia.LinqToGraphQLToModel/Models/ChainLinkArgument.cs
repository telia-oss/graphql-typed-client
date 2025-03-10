using Newtonsoft.Json;

namespace Telia.LinqToGraphQLToModel.Models;

internal class ChainLinkArgument
{
    public string Name { get; set; }
    public object Value { get; set; }
    public string GraphQLType { get; set; }

    public override bool Equals(object obj)
    {
        if (!(obj is ChainLinkArgument))
        {
            return false;
        }

        var arg = obj as ChainLinkArgument;

        return arg.Name == Name && ValuesAreTheSame(arg);
    }

    bool ValuesAreTheSame(ChainLinkArgument arg)
    {
        return JsonConvert.SerializeObject(arg.Value) == JsonConvert.SerializeObject(Value);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
