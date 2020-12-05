using CoreApp.DefensiveCache.Configuration;

namespace CoreApp.DefensiveCache.Interfaces
{
    public interface ICacheServiceMapper
    {
        void Map(CacheConfiguration cacheConfiguration);
    }
}
