![](media/icon.png)

# Strongly typed client for GraphQL in .NET

[![NuGet Badge](https://buildstats.info/nuget/Telia.GraphQL.Client)](https://www.nuget.org/packages/Telia.GraphQL.Client/)
[![Build status](https://ci.appveyor.com/api/projects/status/ftujm98p9t848uh8/branch/master?svg=true)](https://ci.appveyor.com/project/mkmarek/graphql-typed-client/branch/master)

> This project is currently work in progress and contains only very basic functionality. Usage in production is discouraged unless you know what you're doing.

A typesafe way how to request data from GraphQL API.

## First, you generate a model

You can get this Visual Studio [extension](https://marketplace.visualstudio.com/items?itemName=MarekMagdziak.Telia-GraphQL-Tooling)
which automatically generates a cs file out of graphql schema file.

![](media/example.png)

For example this IDL

```graphql
type Query {
	a: Int
	b(x: Int!): Int!
}
```

will be translated into

```csharp
namespace Schema
{
    using System;
    using System.Collections.Generic;
    using Telia.GraphQL.Client.Attributes;

    public class Query
    {
        [GraphQLField("a")]
        public Int32? A
        {
            get;
            set;
        }

        [GraphQLField("b")]
        public Int32 B(Int32 x)
        {
            throw new InvalidOperationException();
        }
    }
}
```

## Create a client based on your schema

This part will be eventually automated with schema generation

```csharp
public class MyClient : GraphQLCLient<Query>
{
    public Client(string endpoint) : base(endpoint)
    {
    }
}
```

## Then request your data using the generated model

Create requests towards your GraphQL API while keeping full intellisense over the schema.

For example:

```csharp
var client = new MyClient("<your_endpoint>");

var data = client.Query(e => new
{
    a = e.A,
    b = e.B(12)
});

Console.Write(data.a);
Console.Write(data.b);
```
