using CoreApp.DefensiveCache.Benchmark.Repositories;
using CoreApp.DefensiveCache.Configuration;
using CoreApp.DefensiveCache.Interfaces;

namespace CoreApp.DefensiveCache.Benchmark
{

    public class CacheServiceMapping : ICacheServiceMapper
    {
        public void Map(CacheConfiguration cacheConfiguration)
        {
            cacheConfiguration.AddCacheService<IDefensiveCacheMapping>(configuration =>
            {
                configuration.AddMethod(x => nameof(x.GetBool), "mapGetBool-{id}", 600);
                configuration.AddMethod(x => nameof(x.GetBoolAsync), "mapGetBoolAsync-{id}", 600);
                configuration.AddMethod(x => nameof(x.GetDateTime), "mapGetDateTime-{id}", 600);
                configuration.AddMethod(x => nameof(x.GetDateTimeAsync), "mapGetDateTimeAsync-{id}", 600);
                configuration.AddMethod(x => nameof(x.GetInt), "mapGetInt-{id}", 600);
                configuration.AddMethod(x => nameof(x.GetIntAsync), "mapGetIntAsync-{id}", 600);
                configuration.AddMethod(x => nameof(x.GetListObject), "mapGetListObject-{filter.Page}_{filter.Records}", 600);
                configuration.AddMethod(x => nameof(x.GetListObjectAsync), "mapGetListObjectAsync-{filter.Page}_{filter.Records}", 600);
                configuration.AddMethod(x => nameof(x.GetObject), "mapGetObject-{id}", 600);
                configuration.AddMethod(x => nameof(x.GetObjectAsync), "mapGetObjectAsync-{id}", 600);
                configuration.AddMethod(x => nameof(x.GetString), "mapGetString-{id}", 600);
                configuration.AddMethod(x => nameof(x.GetStringAsync), "mapGetStringAsync-{id}", 600);
            });
        }
    }
}
