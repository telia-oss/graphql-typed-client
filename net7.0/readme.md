![](../net4.6/media/icon.png)

# C# expressions to GraphQL to C# Models

A typesafe way to create graphql queries from C# expressions

> Looking for the nuget package on the .NET Framework version? It's here: [![NuGet Badge](https://buildstats.info/nuget/Telia.GraphQL.Client)](https://www.nuget.org/packages/Telia.GraphQL.Client/)

## Example
Assume schema

```csharp
namespace Schema
{
    using System;
    using System.Collections.Generic;
    using Telia.GraphQL.Schema.Attributes;

    [GraphQLType("Query")]
    public class SchemaQuery
    {
        [GraphQLField("a", "Int")]
        public int? A { get; set; }

        [GraphQLField("b", "Int")]
        public int B([GraphQLArgument("x", "Int")] int x)
        {
            throw new InvalidOperationException();
        }
    }
}
```

```csharp
var schema = new GraphQLQuery<SchemaQuery>();

var model = schema.Query(() => new {
{
    a = e.A,
    b = e.B(12)
}, Send);

// Variabel 'model' is now the anonymous type with a and b

// Skip sending 'Send' as second argument, to get the graphql query as a string, which prints this:
// {"query":"query Query($var_0:Int){field0:a field1:b(x:$var_0) __typename}","variables":{"var_0":100}}

string Send(string query) {
    // Pseudo code:
    return new HttpClient(url, query, ...).Post().Response;
}
```