using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using CoreApp.DefensiveCache.Benchmark.Models;
using CoreApp.DefensiveCache.Benchmark.Repositories;
#pragma warning disable CS0234 // The type or namespace name 'Serializers' does not exist in the namespace 'CoreApp.DefensiveCache' (are you missing an assembly reference?)
using CoreApp.DefensiveCache.Serializers;
#pragma warning restore CS0234 // The type or namespace name 'Serializers' does not exist in the namespace 'CoreApp.DefensiveCache' (are you missing an assembly reference?)
#pragma warning disable CS0234 // The type or namespace name 'Extensions' does not exist in the namespace 'CoreApp.DefensiveCache' (are you missing an assembly reference?)
using CoreApp.DefensiveCache.Extensions;
#pragma warning restore CS0234 // The type or namespace name 'Extensions' does not exist in the namespace 'CoreApp.DefensiveCache' (are you missing an assembly reference?)
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

            //services.AddDistributedMemoryCache();
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("RedisConnection");
            });

            services.AddScoped<IDefensiveCacheImplemented, TestRepositoryImplemented>();
            services.AddScoped<IDefensiveCacheDynamic, TestRepositoryDynamic>();
            services.AddScoped<IDefensiveCacheMapping, TestRepositoryMapping>();

            services.Decorate<IDefensiveCacheImplemented, DefensiveCacheImplemented>();

            services.AddScoped<ICacheSerializer, JsonNetCacheSerializer>();
            services.DecorateWithCacheServiceMapping(new CacheServiceMapping());
            services.DecorateWithCacheDynamicService<IDefensiveCacheDynamic>((config) =>
            {
                config.AddMethod(x => nameof(x.GetBool), "genGetBool{id}", 600);
                config.AddMethod(x => nameof(x.GetBoolAsync), "genGetBoolAsync{id}", 600);
                config.AddMethod(x => nameof(x.GetDateTime), "genGetDateTime{id}", 600);
                config.AddMethod(x => nameof(x.GetDateTimeAsync), "genGetDateTimeAsync{id}", 600);
                config.AddMethod(x => nameof(x.GetInt), "genGetInt{id}", 600);
                config.AddMethod(x => nameof(x.GetIntAsync), "genGetIntAsync{id}", 600);
                config.AddMethod(x => nameof(x.GetListObject), "genGetListObject{filter.Page}_{filter.Records}", 600);
                config.AddMethod(x => nameof(x.GetListObjectAsync), "genGetListObjectAsync{filter.Page}_{filter.Records}", 600);
                config.AddMethod(x => nameof(x.GetObject), "genGetObject{id}", 600);
                config.AddMethod(x => nameof(x.GetObjectAsync), "genGetObjectAsync{id}", 600);
                config.AddMethod(x => nameof(x.GetString), "genGetString{id}", 600);
                config.AddMethod(x => nameof(x.GetStringAsync), "genGetStringAsync{id}", 600);
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
                var repository = scope.ServiceProvider.GetService<IDefensiveCacheDynamic>();
                await RunTest(repository);
                await RunTest(repository);
            }
        }

        [Benchmark]
        public async Task ProcessDefensiveCacheMapping()
        {
            
            using (var scope = _serviceProvider.CreateScope())
            {
                var repository = scope.ServiceProvider.GetService<IDefensiveCacheMapping>();
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
