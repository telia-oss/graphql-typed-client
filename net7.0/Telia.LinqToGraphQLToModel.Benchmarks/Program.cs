using BenchmarkDotNet.Running;

using Telia.LinqToGraphQL.Benchmarks.LinqToGraphqlBenchmarks.Benchmarks;

BenchmarkRunner.Run(typeof(LinqToGraphqlBenchmarks));