using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading.Tasks;
using CoreApp.DefensiveCache.Extensions;
using CoreApp.DefensiveCache.Tests.Contracts;

namespace CoreApp.DefensiveCache.Tests.Implementations
{
    public class ProductCacheRepository : IProductRepository
    {
        private IProductRepository _concreteRepository;
        private IDistributedCache _distributedCache;

        public ProductCacheRepository(
            IProductRepository concreteRepository, 
            IDistributedCache distributedCache)
        {
            _concreteRepository = concreteRepository;
            _distributedCache = distributedCache;
        }
        public Product GetProduct(int id)
        {
            var cacheKey = $"cache{id}";
            var cacheValue = _distributedCache.GetAsync<Product>(cacheKey).Result;
            if (cacheValue == null)
            {
                cacheValue = _concreteRepository.GetProduct(id);
                var cacheEntryOptions = new DistributedCacheEntryOptions();
                cacheEntryOptions.SetAbsoluteExpiration(DateTime.Now.AddMinutes(1));
                _distributedCache.SetAsync(cacheKey, cacheValue, cacheEntryOptions);
            }
            return cacheValue;
        }

        public async Task<Product> GetProductAsync(int id)
        {
            var cacheKey = $"cacheasync{id}";
            var cacheValue = await _distributedCache.GetAsync<Product>(cacheKey);
            if (cacheValue == null)
            {
                cacheValue = await _concreteRepository.GetProductAsync(id);
                var cacheEntryOptions = new DistributedCacheEntryOptions();
                cacheEntryOptions.SetAbsoluteExpiration(DateTime.Now.AddMinutes(1));
                _distributedCache.SetAsync(cacheKey, cacheValue, cacheEntryOptions).Wait();
            }
            return cacheValue;
        }
    }
}
