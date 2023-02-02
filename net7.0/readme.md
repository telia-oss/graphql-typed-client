![](media/icon.png)

# C# expressions to GraphQL

[![NuGet Badge](https://buildstats.info/nuget/Telia.GraphQL.Client)](https://www.nuget.org/packages/Telia.GraphQL.Client/)

> This project is currently work in progress and contains only very basic functionality. Usage in production is discouraged unless you know what you're doing.

A typesafe way to create graphql queries from C# expressions

## Example

![](media/example.png)

Assume schema

```csharp
namespace Schema
{
    using System;
    using System.Collections.Generic;

    public class Query
    {
        public Int32? A
        {
            get;
            set;
        }

        public Int32 B(Int32 x)
        {
            throw new InvalidOperationException();
        }
    }
}
```

```csharp
var client = new MyClient("<your_endpoint>");

var graphQlQuery = new Query(() => new {
{
    a = e.A,
    b = e.B(12)
});

Console.Write(graphQlQuery); // Prints a formatted graphQlQuery
```