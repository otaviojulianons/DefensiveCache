# DefensiveCache
É uma biblioteca para geração de implementação de cache baseado em interfaces. Sua missão é reduzir aquele bom e velho trecho de código replicado:
```csharp
        public async Task<Entity> GetObjectAsync(int id)
        {
            var value = await _distributedCache.GetAsync<Entity>("GetObjectAsync" + id);
            if(value == null)
            {
                value = await _testRepository.GetObjectAsync(id);
                var cacheEntryOptions = new DistributedCacheEntryOptions();
                cacheEntryOptions.SetAbsoluteExpiration(TimeSpan.FromSeconds(1000));
                await _distributedCache.SetAsync("GetObjectAsync" + id, value, cacheEntryOptions);
            }
            return value;
        }
```

## Funcionamento
O DefensiveCache utiliza [__Reflection__](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/reflection) para mapear a interface do serviço concreto, a partir dessa estrutura é realizado a compilação de uma classe concreta utilizando o compilador [__Roslyn__](https://github.com/dotnet/roslyn), a implementação gerada é baseado _design pattern_ [__Decorator__](https://refactoring.guru/design-patterns/decorator), configurada por injeção de dependência utilizando o [__Scrutor__](https://github.com/khellang/Scrutor). Os métodos que não possuirem cache, serão direcionados diretamente para implementação concreta.

## Importante
O DefensiveCache está preparado para mapear interfaces que tenham __apenas__ métodos, interfaces com qualquer propriedade __NÃO__ serão mapeadas nessa versão.

## Como usar
No Startup da aplicação, realizamos a configuração do serviço de formatação de cache _ICacheFormatter_ e o mapeamento dos métodos de cache. Utilize {chaves} para realizar a interpolação de valores na criação das chaves de cache.

```csharp
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            services.AddScoped<ICacheFormatter, JsonNetCacheFormatter>();
            services.DecorateWithCacheGenerated<EnitityRepository>((config) =>
            {
                config.AddMethod(x => nameof(x.GetObjectAsync), "genGetObjectAsync{id}", 60);
            });
        }
```

## Benchmark
``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18362.959 (1903/May2019Update/19H1)
Intel Core i7-8565U CPU 1.80GHz (Whiskey Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.1.302
  [Host]     : .NET Core 3.1.6 (CoreCLR 4.700.20.26901, CoreFX 4.700.20.31603), X64 RyuJIT
  DefaultJob : .NET Core 3.1.6 (CoreCLR 4.700.20.26901, CoreFX 4.700.20.31603), X64 RyuJIT


```
|                            Method |      Mean |    Error |   StdDev | Completed Work Items | Lock Contentions |   Gen 0 | Gen 1 | Gen 2 | Allocated | Allocated native memory | Native memory leak |
|---------------------------------- |----------:|---------:|---------:|---------------------:|-----------------:|--------:|------:|------:|----------:|------------------------:|-------------------:|
|  ProcessDefensiveCacheImplemented | 140.24 μs | 2.799 μs | 4.677 μs |               0.0005 |                - | 28.0762 |     - |     - | 116.13 KB |                       - |                  - |
|    ProcessDefensiveCacheGenerated |  45.96 μs | 0.406 μs | 0.360 μs |               0.0001 |                - |  4.8828 |     - |     - |  20.18 KB |                       - |                  - |
| ProcessDefensiveCacheDynamicProxy | 730.32 μs | 5.215 μs | 4.878 μs |               0.0039 |                - | 70.3125 |     - |     - | 293.76 KB |                    0 KB |                  - |

