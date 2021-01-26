using Microsoft.Extensions.Caching.Distributed;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoreApp.DefensiveCache.Extensions
{
    public static class DistributedCacheExtensions
    {
        public static void Set<T>(this IDistributedCache cache, string key, T value, DistributedCacheEntryOptions options)
        {
            if (Equals(value, default(T)))
                return;

            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, value);
                bytes = memoryStream.ToArray();
            }

            cache.Set(key, bytes, options);
        }

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
            var bytes = await cache.GetAsync(key);
            if (bytes == null)
                return default;

            using (var memoryStream = new MemoryStream(bytes))
            {
                var binaryFormatter = new BinaryFormatter();
                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }

        public static T Get<T>(this IDistributedCache cache, string key)
        {
            var bytes = cache.Get(key);
            if (bytes == null) 
                return default;

            using (var memoryStream = new MemoryStream(bytes))
            {
                var binaryFormatter = new BinaryFormatter();
                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }
    }
}