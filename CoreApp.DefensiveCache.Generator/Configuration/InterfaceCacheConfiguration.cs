using System.Collections.Generic;
using System.Linq;

namespace CoreApp.DefensiveCache.Configuration
{
    public class InterfaceCacheConfiguration
    {
        public InterfaceCacheConfiguration()
        {
            Methods = new List<MethodCacheConfiguration>();
        }

        public InterfaceCacheConfiguration(string name): this()
        { 
            Name = name;
        }

        public MethodCacheConfiguration this[string methodName]
        {
            get => Methods?.FirstOrDefault(x => x.Name == methodName);
        }

        public string Name { get; set; }
        public List<MethodCacheConfiguration> Methods { get; set; }

        public void AddMethod(string name, string key, double expirationSeconds)
        {
            var method = new MethodCacheConfiguration()
            {
                Name = name,
                KeyTemplate = key,
                ExpirationSeconds = expirationSeconds
            };
            Methods.Add(method);
        }

        public void Merge(InterfaceCacheConfiguration newCacheConfiguration)
        {
            foreach (var methodCacheConfiguration in newCacheConfiguration?.Methods ?? new List<MethodCacheConfiguration>())
            {
                var matchedMethod = this[methodCacheConfiguration.Name];
                if (matchedMethod == null)
                    return;
                matchedMethod.ExpirationSeconds = methodCacheConfiguration.ExpirationSeconds;
            }
        }
    }
}
