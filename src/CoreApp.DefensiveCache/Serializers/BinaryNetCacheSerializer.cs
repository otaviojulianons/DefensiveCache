using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace CoreApp.DefensiveCache.Serializers
{
    public class BinaryNetCacheSerializer : ICacheSerializer
    {
        private IDistributedCache _distributedCache;
        private ILogger<BinaryNetCacheSerializer> _logger;

        public BinaryNetCacheSerializer(IDistributedCache distributedCache, ILogger<BinaryNetCacheSerializer> logger)
        {
            _distributedCache = distributedCache;
            _logger = logger;
        }

        public T Get<T>(string key)
        {
            _logger.LogInformation($"Get key:{key} type:{typeof(T).Name}");
            var data = _distributedCache.Get(key);
            if (data == null)
                return default;

            using (var memoryStream = new MemoryStream(data))
            {
                var binaryFormatter = new BinaryFormatter();
                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }

        public async Task<T> GetAsync<T>(string key)
        {
            _logger.LogInformation($"GetAsync key:{key} type:{typeof(T).Name}");
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
                _distributedCache.Set(key, dataArray, cacheEntryOptions);
            }
        }

        public async Task SetAsync<T>(string key, T data, double expirationSeconds)
        {
            _logger.LogInformation($"SetAsync key:{key} with {expirationSeconds}s type:{typeof(T).Name}");
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
