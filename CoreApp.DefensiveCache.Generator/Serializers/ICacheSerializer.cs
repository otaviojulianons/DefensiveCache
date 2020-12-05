using System.Threading.Tasks;

namespace CoreApp.DefensiveCache.Serializers
{
    public interface ICacheSerializer
    {
        T Get<T>(string key);

        Task<T> GetAsync<T>(string key);

        void Set<T>(string key, T data, double expirationSeconds);

        Task SetAsync<T>(string key, T data, double expirationSeconds);
    }
}
