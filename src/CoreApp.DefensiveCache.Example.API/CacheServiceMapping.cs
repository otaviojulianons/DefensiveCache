using CoreApp.DefensiveCache.Configuration;
using CoreApp.DefensiveCache.Interfaces;
using CoreApp.DefensiveCache.Tests.Contracts;

namespace CoreApp.DefensiveCache.Example.API
{

    public class CacheServiceMapping : ICacheServiceMapper
    {
        public void Map(CacheConfiguration cacheConfiguration)
        {
            cacheConfiguration.AddCacheService<IProductRepository>(configuration =>
            {
                configuration.AddMethod(x => nameof(x.GetProduct), "prod-{id}", 60);
                configuration.AddMethod(x => nameof(x.GetProductAsync), "prod-async-{id}", 60);
            });
        }
    }
}
