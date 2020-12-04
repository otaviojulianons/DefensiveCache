namespace CoreApp.DefensiveCache.Configuration.Core
{
    public class MethodCacheConfiguration
    {
        public string Name { get; set; }
        public string KeyTemplate { get; set; }
        public double ExpirationSeconds { get; set; }
    }
}
