using System.Reflection;

namespace CoreApp.DefensiveCache.Templates
{
    public class CachePropertyTemplate
    {
        public CachePropertyTemplate(PropertyInfo propertyInfo)
        {
            Name = propertyInfo.Name;
            CanRead = propertyInfo.CanRead;
            CanWrite = propertyInfo.CanWrite;
        }

        public string Name { get; private set; }
        public bool CanRead { get; private set; }
        public bool CanWrite { get; private set; }
    }
}
