﻿using CoreApp.DefensiveCache.Configuration.Core;
using CoreApp.DefensiveCache.Core.Interfaces;
using CoreApp.DefensiveCache.Tests.Contracts;

namespace CoreApp.DefensiveCache.Example
{
    public class CacheServiceMapping : ICacheServiceMapper
    {
        public void Map(CacheConfiguration cacheConfiguration)
        {
            cacheConfiguration.AddCacheService<IProductRepository>(configuration =>
            {
                configuration.AddMethod(x => nameof(x.GetProduct), "prod{id}", 60);
            });
        }
    }
}