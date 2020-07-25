using System.Collections.Generic;

namespace CoreApp.DefensiveCache.Templates
{
    public class CacheTemplate
    {
        private List<CacheMethodTemplate> _methods = new List<CacheMethodTemplate>();

        public string Name { get; set; }

        public string InterfaceName { get; set; }

        public IReadOnlyCollection<CacheMethodTemplate> Methods => _methods;

        public void AddMethod(CacheMethodTemplate cacheMethodTemplate) => _methods.Add(cacheMethodTemplate);
    }
}
