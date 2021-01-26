using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreApp.DefensiveCache.Configuration
{
    public class CacheConfiguration
    {
        public CacheConfiguration()
        {
            Services = new List<InterfaceCacheConfiguration>();
        }

        public InterfaceCacheConfiguration this[string serviceName]
        {
            get => Services?.FirstOrDefault(x => x.Name == serviceName);
        }


        public List<InterfaceCacheConfiguration> Services { get; set; }

        public void AddCacheService<T>(Action<InterfaceCacheConfigurationTyped<T>> configure)
        {
            var configuration = new InterfaceCacheConfigurationTyped<T>();
            configure(configuration);
            Services.Add(configuration);
        }
    }
}
