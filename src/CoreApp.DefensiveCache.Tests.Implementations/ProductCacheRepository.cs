using CoreApp.DefensiveCache.Extensions;
using CoreApp.DefensiveCache.Serializers;
using CoreApp.DefensiveCache.Tests.Contracts;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace CoreApp.DefensiveCache.Tests.Implementations
{
    public class ProductCacheRepository : IProductRepository
    {
        private IProductRepository _concreteRepository;
        private IDistributedCache _distributedCache;
        private IConfiguration _cacheConfiguration;

        public ProductCacheRepository(
            ICacheSerializer cacheSerializer,
            IConfiguration configuration,
            IProductRepository concreteRepository, 
            IDistributedCache distributedCache)
        {
            _concreteRepository = concreteRepository;
            _distributedCache = distributedCache;
            _cacheConfiguration = configuration.GetSection("Cache:Services:IProductRepository");
            
        }
        public Product GetProduct(int id)
        {
            var expirationGetProduct = _cacheConfiguration.GetValue("GetProduct:ExpirationSeconds", 5);
            if (expirationGetProduct == 0)
                return _concreteRepository.GetProduct(id);

            var cacheKey = $"cache{id}";
            var cacheValue = _distributedCache.GetAsync<Product>(cacheKey).Result;
            if (cacheValue == null)
            {
                cacheValue = _concreteRepository.GetProduct(id);
                var cacheEntryOptions = new DistributedCacheEntryOptions();
                cacheEntryOptions.SetAbsoluteExpiration(DateTime.Now.AddSeconds(expirationGetProduct));
                _distributedCache.SetAsync(cacheKey, cacheValue, cacheEntryOptions);
            }
            return cacheValue;
        }

        public async Task<Product> GetProductAsync(int id)
        {
            var expirationGetProduct = _cacheConfiguration.GetValue("GetProductAsyncExpirationSeconds", 3);
            if (expirationGetProduct == 0)
                return _concreteRepository.GetProduct(id);

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
