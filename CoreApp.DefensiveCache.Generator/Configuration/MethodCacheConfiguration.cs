namespace CoreApp.DefensiveCache.Configuration
{
    public class MethodCacheConfiguration
    {
        public string Name { get; set; }
        public string KeyTemplate { get; set; }
        public double ExpirationSeconds { get; set; }
    }
}
