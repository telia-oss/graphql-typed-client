using BenchmarkDotNet.Running;

using Telia.LinqToGraphQLToModel.Benchmarks.LinqToGraphqlBenchmarks.Benchmarks;

BenchmarkRunner.Run(typeof(LinqToGraphqlBenchmarks));