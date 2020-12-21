using CoreApp.DefensiveCache.Benchmark.Models;
using CoreApp.DefensiveCache.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreApp.DefensiveCache.Benchmark.Repositories
{
    public class DefensiveCacheImplemented : IDefensiveCacheImplemented
    {
        private const int SegundosCache = 600;
        private ITestRepository _testRepository;
        private IDistributedCache _distributedCache;

        public DefensiveCacheImplemented(
            ITestRepository repository, 
            IDistributedCache distributedCache)
        {
            _testRepository = repository;
            _distributedCache = distributedCache;
        }

        public async Task<Entity> GetObjectAsync(int id)
        {
            var value = await _distributedCache.GetAsync<Entity>("GetObjectAsync" + id);
            if(value == null)
            {
                value = await _testRepository.GetObjectAsync(id);
                var cacheEntryOptions = new DistributedCacheEntryOptions();
                cacheEntryOptions.SetAbsoluteExpiration(TimeSpan.FromSeconds(SegundosCache));
                await _distributedCache.SetAsync("GetObjectAsync" + id, value, cacheEntryOptions);
            }
            return value;
        }

        public Entity GetObject(int id)
        {
            var value = _distributedCache.Get<Entity>("GetObject");
            if (value == null)
            {
                value = _testRepository.GetObject(id);
                var cacheEntryOptions = new DistributedCacheEntryOptions();
                cacheEntryOptions.SetAbsoluteExpiration(TimeSpan.FromSeconds(SegundosCache));
                _distributedCache.Set("GetObject", value, cacheEntryOptions);
            }
            return value;
        }

        public async Task<List<Entity>> GetListObjectAsync(ParamQuery filter)
        {
            var value = await _distributedCache.GetAsync<List<Entity>>("GetListObjectAsync" + filter.Page + filter.Records);
            if (value == null)
            {
                value = await _testRepository.GetListObjectAsync(filter);
                var cacheEntryOptions = new DistributedCacheEntryOptions();
                cacheEntryOptions.SetAbsoluteExpiration(TimeSpan.FromSeconds(SegundosCache));
                await _distributedCache.SetAsync("GetListObjectAsync" + filter.Page + filter.Records, value, cacheEntryOptions);
            }
            return value;
        }

        public List<Entity> GetListObject(ParamQuery filter)
        {
            var value = _distributedCache.Get<List<Entity>>("GetListObject" + filter.Page + filter.Records);
            if (value == null)
            {
                value = _testRepository.GetListObject(filter);
                var cacheEntryOptions = new DistributedCacheEntryOptions();
                cacheEntryOptions.SetAbsoluteExpiration(TimeSpan.FromSeconds(SegundosCache));
                _distributedCache.Set("GetListObject" + filter.Page + filter.Records, value, cacheEntryOptions);
            }
            return value;
        }

        public async Task<string> GetStringAsync(int id)
        {
            var value = await _distributedCache.GetAsync<string>("GetStringAsync_" + id);
            if (value == null)
            {
                value = await _testRepository.GetStringAsync(id);
                var cacheEntryOptions = new DistributedCacheEntryOptions();
                cacheEntryOptions.SetAbsoluteExpiration(TimeSpan.FromSeconds(SegundosCache));
                await _distributedCache.SetAsync("GetStringAsync_" + id, value, cacheEntryOptions);
            }
            return value;
        }

        public string GetString(int id)
        {
            var value = _distributedCache.Get<string>("GetString" + id);
            if (value == null)
            {
                value = _testRepository.GetString(id);
                var cacheEntryOptions = new DistributedCacheEntryOptions();
                cacheEntryOptions.SetAbsoluteExpiration(TimeSpan.FromSeconds(SegundosCache));
                _distributedCache.Set("GetString" + id, value, cacheEntryOptions);
            }
            return value;
        }

        public async Task<int?> GetIntAsync(int id)
        {
            var value = await _distributedCache.GetAsync<int?>("GetIntAsync" + id);
            if (Equals(value, default(int?)))
            {
                value = await _testRepository.GetIntAsync(id);
                var cacheEntryOptions = new DistributedCacheEntryOptions();
                cacheEntryOptions.SetAbsoluteExpiration(TimeSpan.FromSeconds(SegundosCache));
                await _distributedCache.SetAsync("GetIntAsync" + id, value, cacheEntryOptions);
            }
            return value;
        }

        public int? GetInt(int id)
        {
            var value = _distributedCache.Get<int?>("GetInt" + id);
            if (Equals(value, default(int?)))
            {
                value = _testRepository.GetInt(id);
                var cacheEntryOptions = new DistributedCacheEntryOptions();
                cacheEntryOptions.SetAbsoluteExpiration(TimeSpan.FromSeconds(SegundosCache));
                _distributedCache.Set("GetInt" + id, value, cacheEntryOptions);
            }
            return value;
        }

        public async Task<DateTime> GetDateTimeAsync(int id)
        {
            var value = await _distributedCache.GetAsync<DateTime>("GetDateTimeAsync" + id);
            if (Equals(value, default(DateTime)))
            {
                value = await _testRepository.GetDateTimeAsync(id);
                var cacheEntryOptions = new DistributedCacheEntryOptions();
                cacheEntryOptions.SetAbsoluteExpiration(TimeSpan.FromSeconds(SegundosCache));
                await _distributedCache.SetAsync("GetDateTimeAsync" + id, value, cacheEntryOptions);
            }
            return value;
        }

        public DateTime GetDateTime(int id)
        {
            var value = _distributedCache.Get<DateTime>("GetDateTime" + id);
            if (Equals(value, default(DateTime)))
            {
                value = _testRepository.GetDateTime(id);
                var cacheEntryOptions = new DistributedCacheEntryOptions();
                cacheEntryOptions.SetAbsoluteExpiration(TimeSpan.FromSeconds(SegundosCache));
                _distributedCache.Set("GetDateTime" + id, value, cacheEntryOptions);
            }
            return value;
        }

        public async Task<bool> GetBoolAsync(int id)
        {
            var value = await _distributedCache.GetAsync<bool>("GetBoolAsync" + id);
            if (Equals(value, default(bool)))
            {
                value = await _testRepository.GetBoolAsync(id);
                var cacheEntryOptions = new DistributedCacheEntryOptions();
                cacheEntryOptions.SetAbsoluteExpiration(TimeSpan.FromSeconds(SegundosCache));
                await _distributedCache.SetAsync("GetBoolAsync" + id, value, cacheEntryOptions);
            }
            return value;
        }

        public bool GetBool(int id)
        {
            var value = _distributedCache.Get<bool>("GetBool" + id);
            if (Equals(value,default(bool)))
            {
                value = _testRepository.GetBool(id);
                var cacheEntryOptions = new DistributedCacheEntryOptions();
                cacheEntryOptions.SetAbsoluteExpiration(TimeSpan.FromSeconds(SegundosCache));
                _distributedCache.Set("GetBool" + id, value, cacheEntryOptions);
            }
            return value;
        }

    }
}