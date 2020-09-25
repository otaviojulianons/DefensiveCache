using System;

namespace CoreApp.DefensiveCache.Configuration.Core
{
    public class InterfaceCacheConfigurationTyped<T> : InterfaceCacheConfiguration
    {
        public InterfaceCacheConfigurationTyped() : base(typeof(T).Name)
        {
        }

        public void AddMethod(Func<T, string> getName, string key, double expirationSeconds)
        {
            base.AddMethod(getName(default(T)), key, expirationSeconds);
        }
    }
}
