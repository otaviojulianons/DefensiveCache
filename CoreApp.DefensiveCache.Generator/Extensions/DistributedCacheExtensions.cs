using Microsoft.Extensions.Caching.Distributed;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace CoreApp.DefensiveCache.Extensions.Core
{
    public static class DistributedCacheExtensions
    {
        public static Task SetAsync<T>(this IDistributedCache cache, string key, T value, DistributedCacheEntryOptions options)
        {
            if (Equals(value, default(T)))
                return Task.CompletedTask;

            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, value);
                bytes = memoryStream.ToArray();
            }

            return cache.SetAsync(key, bytes, options);
        }

        public static async Task<T> GetAsync<T>(this IDistributedCache cache, string key)
        {
            var val = await cache.GetAsync(key);
            var result = default(T);

            if (val == null) return result;

            using (var memoryStream = new MemoryStream(val))
            {
                var binaryFormatter = new BinaryFormatter();
                result = (T)binaryFormatter.Deserialize(memoryStream);
            }

            return result;
        }
    }
}