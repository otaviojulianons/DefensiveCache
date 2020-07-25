using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using CoreApp.DefensiveCache.Benchmark.Models;
using CoreApp.DefensiveCache.Benchmark.Repositories;
using CoreApp.DefensiveCache.Serializers;
using CoreApp.DefensiveCache.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace CoreApp.DefensiveCache.Benchmark
{
    [MemoryDiagnoser]
    [ConcurrencyVisualizerProfiler]
    [NativeMemoryProfiler]
    [ThreadingDiagnoser]
    public class Benchmark
    {
        private ServiceProvider _serviceProvider;

        public Benchmark()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();

            var services = new ServiceCollection();
            services.AddLogging(cfg => cfg.AddConsole());
            services.AddSingleton<IConfiguration>(configuration);
            services.AddScoped<ICacheSerializer, JsonNetCacheSerializer>();
            services.AddScoped<IDefensiveCacheImplemented, TestRepository1>();
            services.AddScoped<IDefensiveCacheGenerated, TestRepository2>();
            services.AddScoped<IDefensiveCacheDynamicProxy, TestRepository3>();
            services.Decorate<IDefensiveCacheImplemented, DefensiveCacheImplemented>();

            services.AddDistributedMemoryCache();

            services.DecorateWithCacheGenerated<IDefensiveCacheGenerated>((config) =>
            {
                config.AddMethod(x => nameof(x.GetBool), "genGetBool{id}", 60);
                config.AddMethod(x => nameof(x.GetBoolAsync), "genGetBoolAsync{id}", 60);
                config.AddMethod(x => nameof(x.GetDateTime), "genGetDateTime{id}", 60);
                config.AddMethod(x => nameof(x.GetDateTimeAsync), "genGetDateTimeAsync{id}", 60);
                config.AddMethod(x => nameof(x.GetInt), "genGetInt{id}", 60);
                config.AddMethod(x => nameof(x.GetIntAsync), "genGetIntAsync{id}", 60);
                config.AddMethod(x => nameof(x.GetListObject), "genGetListObject{filter.Page}_{filter.Records}", 60);
                config.AddMethod(x => nameof(x.GetListObjectAsync), "genGetListObjectAsync{filter.Page}_{filter.Records}", 60);
                config.AddMethod(x => nameof(x.GetObject), "genGetObject{id}", 60);
                config.AddMethod(x => nameof(x.GetObjectAsync), "genGetObjectAsync{id}", 60);
                config.AddMethod(x => nameof(x.GetString), "genGetString{id}", 60);
                config.AddMethod(x => nameof(x.GetStringAsync), "genGetStringAsync{id}", 60);
            });

            services.DecorateWithCacheProxy<IDefensiveCacheDynamicProxy>((config) =>
            {
                config.AddMethod(x => nameof(x.GetBool), "proxyGetBool{{id}}", 60);
                config.AddMethod(x => nameof(x.GetBoolAsync), "proxyGetBoolAsync{{id}}", 60);
                config.AddMethod(x => nameof(x.GetDateTime), "proxyGetDateTime{{id}}", 60);
                config.AddMethod(x => nameof(x.GetDateTimeAsync), "proxyGetDateTimeAsync{{id}}", 60);
                config.AddMethod(x => nameof(x.GetInt), "proxyGetInt{{id}}", 60);
                config.AddMethod(x => nameof(x.GetIntAsync), "proxyGetIntAsync{{id}}", 60);
                config.AddMethod(x => nameof(x.GetListObject), "proxyGetListObject{{filter.Page}}_{{filter.Records}}", 60);
                config.AddMethod(x => nameof(x.GetListObjectAsync), "proxyGetListObjectAsync{{filter.Page}}_{{filter.Records}}", 60);
                config.AddMethod(x => nameof(x.GetObject), "proxyGetObject{{id}}", 60);
                config.AddMethod(x => nameof(x.GetObjectAsync), "proxyGetObjectAsync{{id}}", 60);
                config.AddMethod(x => nameof(x.GetString), "proxyGetString{{id}}", 60);
                config.AddMethod(x => nameof(x.GetStringAsync), "proxyGetStringAsync{{id}}", 60);
            });

            _serviceProvider = services.BuildServiceProvider();
        }

        [Benchmark]
        public async Task ProcessDefensiveCacheImplemented()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var repository = scope.ServiceProvider.GetService<IDefensiveCacheImplemented>();
                await RunTest(repository);
                await RunTest(repository);
            }
        }

        [Benchmark]
        public async Task ProcessDefensiveCacheGenerated()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var repository = scope.ServiceProvider.GetService<IDefensiveCacheGenerated>();
                await RunTest(repository);
                await RunTest(repository);
            }
        }

        [Benchmark]
        public async Task ProcessDefensiveCacheDynamicProxy()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var repository = scope.ServiceProvider.GetService<IDefensiveCacheDynamicProxy>();
                await RunTest(repository);
                await RunTest(repository);
            }
        }

        private async Task RunTest(ITestRepository repository)
        {
            var result1 = repository.GetBool(1);
            var result2 = await repository.GetBoolAsync(1);
            var result3 = repository.GetDateTime(1);
            var result4 = await repository.GetDateTimeAsync(1);
            var result5 = repository.GetInt(1);
            var result6 = await repository.GetIntAsync(1);
            var result7 = repository.GetListObject(new ParamQuery() { Page = 1, Records = 2 });
            var result8 = await repository.GetListObjectAsync(new ParamQuery() { Page = 2, Records = 4 });
            var result9 = repository.GetObject(1);
            var result10 = await repository.GetObjectAsync(1);
            var result11 = repository.GetString(1);
            var result12 = await repository.GetStringAsync(1);
        }
    }
}
