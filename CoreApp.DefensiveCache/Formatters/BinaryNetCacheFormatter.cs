using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace CoreApp.DefensiveCache.Formatters
{
    public class BinaryNetCacheFormatter : ICacheFormatter
    {
        private IDistributedCache _distributedCache;
        private ILogger<BinaryNetCacheFormatter> _logger;

        public BinaryNetCacheFormatter(IDistributedCache distributedCache, ILogger<BinaryNetCacheFormatter> logger)
        {
            _distributedCache = distributedCache;
            _logger = logger;
        }

        public T Get<T>(string key)
        {
            return GetAsync<T>(key).Result;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            _logger.LogInformation($"Get key:{key} type:{typeof(T).Name}");
            var data = await _distributedCache.GetAsync(key);
            if (data == null)
                return default;

            using (var memoryStream = new MemoryStream(data))
            {
                var binaryFormatter = new BinaryFormatter();
                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }

        public void Set<T>(string key, T data, double expirationSeconds)
        {
            SetAsync(key, data, expirationSeconds).Wait();
        }

        public async Task SetAsync<T>(string key, T data, double expirationSeconds)
        {
            _logger.LogInformation($"Set key:{key} with {expirationSeconds}s type:{typeof(T).Name}");
            if (data == null)
                return;

            using (var memoryStream = new MemoryStream())
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, data);
                var dataArray = memoryStream.ToArray();
                var cacheEntryOptions = new DistributedCacheEntryOptions();
                cacheEntryOptions.SetAbsoluteExpiration(TimeSpan.FromSeconds(expirationSeconds));
                await _distributedCache.SetAsync(key, dataArray, cacheEntryOptions);
            }
        }
    }
}
