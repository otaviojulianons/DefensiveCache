using System;

namespace CoreApp.DefensiveCache.Configuration.Core
{
    public class InterfaceCacheConfigurationTyped<T> : InterfaceCacheConfigurationTyped
    {
        public InterfaceCacheConfigurationTyped() : base(typeof(T))
        {
        }

        public void AddMethod(Func<T, string> getName, string key, double expirationSeconds = 0)
        {
            base.AddMethod(getName(default(T)), key, expirationSeconds);
        }
    }

    public class InterfaceCacheConfigurationTyped : InterfaceCacheConfiguration
    {
        public InterfaceCacheConfigurationTyped(Type type) : base(type.Name)
        {
            ServiceType = type;
        }

        public Type ServiceType { get; set; }
    }
}
