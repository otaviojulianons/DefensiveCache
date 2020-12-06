using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using CoreApp.DefensiveCache;
using CoreApp.DefensiveCache.Serializers;
using CoreApp.DefensiveCache.Extensions;
using Microsoft.Extensions.Configuration;

/********* Hey You! *********
 *
 * This is a generated file and is rewritten every time you build the project. Mappings are located in the ICacheServiceMapper.
 * 
 ****************************/

namespace DynamicAssembly
{
    public class IProductRepositoryDynamicCache : CoreApp.DefensiveCache.Tests.Contracts.IProductRepository
    {
        private CoreApp.DefensiveCache.Tests.Contracts.IProductRepository _repository;
        private ICacheSerializer _cacheSerializer;
        private IConfiguration _cacheConfiguration;

        public IProductRepositoryDynamicCache(
            CoreApp.DefensiveCache.Tests.Contracts.IProductRepository repository, 
            IConfiguration configuration,
            ICacheSerializer cacheSerializer)
        {
            _repository = repository;
            _cacheSerializer = cacheSerializer;
            _cacheConfiguration = configuration.GetSection("Cache:Services:IProductRepository");
        }


        public CoreApp.DefensiveCache.Tests.Contracts.Product GetProduct(System.Int32 id)
        {
            var expirationGetProduct = _cacheConfiguration.GetValue("GetProduct:ExpirationSeconds", 60);
            if (expirationGetProduct == 0)
                return _repository.GetProduct(id);

            var cacheKey = $"prod{id}";
            var cacheValue = _cacheSerializer.Get<CoreApp.DefensiveCache.Tests.Contracts.Product>(cacheKey);
            if (Equals(cacheValue, default(CoreApp.DefensiveCache.Tests.Contracts.Product)))
            {
                cacheValue = _repository.GetProduct(id);
                _cacheSerializer.Set(cacheKey, cacheValue, expirationGetProduct);
            }
            return cacheValue;
        }

        public async System.Threading.Tasks.Task<CoreApp.DefensiveCache.Tests.Contracts.Product> GetProductAsync(System.Int32 id)
        {
            var expirationGetProductAsync = _cacheConfiguration.GetValue("GetProductAsync:ExpirationSeconds", 60);
            if (expirationGetProductAsync == 0)
                return await _repository.GetProductAsync(id);

            var cacheKey = $"prod-async{id}";
            var cacheValue = await _cacheSerializer.GetAsync<CoreApp.DefensiveCache.Tests.Contracts.Product>(cacheKey);
            if (Equals(cacheValue, default(CoreApp.DefensiveCache.Tests.Contracts.Product)))
            {
                cacheValue = await _repository.GetProductAsync(id);
                _cacheSerializer.SetAsync(cacheKey, cacheValue, expirationGetProductAsync);
            }
            return cacheValue;
        }
    }

}

