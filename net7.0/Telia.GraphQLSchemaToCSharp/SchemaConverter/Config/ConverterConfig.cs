using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Telia.GraphQLSchemaToCSharp.Config;

public class ConverterConfig
{
    Dictionary<string, Type> graphQLToCSharpTypeBindings;

    public ConverterConfig()
    {
        SetupDefaults();
    }

    void SetupDefaults()
    {
        graphQLToCSharpTypeBindings = new Dictionary<string, Type>
        {
            { "Int", typeof(int) },
            { "Float", typeof(float) },
            { "Boolean", typeof(bool) },
            { "String", typeof(string) },
            { "AWSDate", typeof(DateTime) },
            { "AWSTime", typeof(TimeSpan) },
            { "AWSDateTime", typeof(DateTime) },
            { "AWSTimestamp", typeof(int) },
            { "AWSEmail", typeof(string) },
            { "AWSJSON", typeof(string) },
            { "AWSURL", typeof(string) },
            { "AWSPhone", typeof(string) },
            { "AWSIPAddress", typeof(string) },
            { "ID", typeof(string) },
            { "DateTime", typeof(DateTime) }
        };
    }

    public TypeSyntax GetCSharpTypeFromGraphQLType(string graphQLType, bool nullable)
    {
        if (!graphQLToCSharpTypeBindings.ContainsKey(graphQLType))
        {
            return null;
        }

        var type = graphQLToCSharpTypeBindings[graphQLType];

        if (type.IsValueType && nullable)
        {
            var typeName = SyntaxFactory.ParseTypeName(type.Name);

            return SyntaxFactory.NullableType(typeName);
        }

        return SyntaxFactory.ParseTypeName(type.Name);
    }
}
