using System.Collections.Generic;

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
    }
}
