using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

using Test;

namespace Telia.LinqToGraphQL.Benchmarks;

[SimpleJob(RuntimeMoniker.Net70, warmupCount: 3, launchCount: 3, iterationCount: 5, invocationCount: 20)]
[MemoryDiagnoser]
[RPlotExporter]
public class LinqToGraphQLBenchmarks
{
    [GlobalSetup]
    public void Setup()
    {
        var service = new CountryQueries();

        var countries = service.GetCountries();

        var country = service.GetCountry("KR");

        if (country == null || countries == null)
            throw new Exception("");
    }

    [Benchmark]
    public string GetAllCountries()
    {
        var service = new CountryQueries();

        var countries = service.GetCountries();

        return countries;
    }

    [Benchmark]
    public string GetCountry()
    {
        var service = new CountryQueries();

        var country = service.GetCountry("KR");

        Dump.Write(country);

        return country;
    }
}
