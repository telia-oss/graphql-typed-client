``` ini

BenchmarkDotNet=v0.13.4, OS=Windows 10 (10.0.19045.2486)
Intel Core i7-9750H CPU 2.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK=7.0.102
  [Host]     : .NET 7.0.2 (7.0.222.60605), X64 RyuJIT AVX2
  Job-FIOLYL : .NET 7.0.2 (7.0.222.60605), X64 RyuJIT AVX2

Runtime=.NET 7.0  InvocationCount=20  IterationCount=5  
LaunchCount=3  UnrollFactor=1  WarmupCount=3  

```
|          Method |     Mean |     Error |    StdDev | Allocated |
|---------------- |---------:|----------:|----------:|----------:|
| GetAllCountries | 133.8 μs |  12.71 μs |  10.61 μs |  33.17 KB |
|      GetCountry | 789.0 μs | 108.54 μs | 101.53 μs |  24.54 KB |
