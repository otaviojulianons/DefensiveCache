using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreApp.DefensiveCache.Serializers
{
    public class JsonNetCacheSerializer : ICacheSerializer
    {
        private IDistributedCache _distributedCache;
        private ILogger<JsonNetCacheSerializer> _logger;

        public JsonNetCacheSerializer(IDistributedCache distributedCache, ILogger<JsonNetCacheSerializer> logger)
        {
            _distributedCache = distributedCache;
            _logger = logger;
        }

        public T Get<T>(string key)
        {
            _logger.LogInformation($"Get key:{key} type:{typeof(T).Name}");
            var data = _distributedCache.GetString(key);
            if (data == null)
                return default;

            return JsonSerializer.Deserialize<T>(data);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            _logger.LogInformation($"GetAsync key:{key} type:{typeof(T).Name}");
            var data = await _distributedCache.GetStringAsync(key);
            if (data == null)
                return await Task.FromResult(default(T));

            return await Task.FromResult(JsonSerializer.Deserialize<T>(data));
        }

        public void Set<T>(string key, T data, double expirationSeconds)
        {
            _logger.LogInformation($"Set key:{key} with {expirationSeconds}s type:{typeof(T).Name}");
            var dataString = JsonSerializer.Serialize(data);
            var cacheEntryOptions = new DistributedCacheEntryOptions();
            cacheEntryOptions.SetAbsoluteExpiration(TimeSpan.FromSeconds(expirationSeconds));
            _distributedCache.SetString(key, dataString, cacheEntryOptions);
        }

        public async Task SetAsync<T>(string key, T data, double expirationSeconds)
        {
            _logger.LogInformation($"SetAsync key:{key} with {expirationSeconds}s type:{typeof(T).Name}");
            var dataString = JsonSerializer.Serialize<T>(data);
            var cacheEntryOptions = new DistributedCacheEntryOptions();
            cacheEntryOptions.SetAbsoluteExpiration(TimeSpan.FromSeconds(expirationSeconds));
            await _distributedCache.SetStringAsync(key, dataString, cacheEntryOptions);
        }

        public T Deserialize<T>(string data)
        {
            return data == null ? default : JsonSerializer.Deserialize<T>(data);
        }

        public Task<T> DeserializeAsync<T>(string data)
        {
            return Task.FromResult(Deserialize<T>(data));
        }

    }
}
