using System;

namespace CoreApp.DefensiveCache.Benchmark.Models
{
    [Serializable]
    public class Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ParamQuery
    {
        public int Page { get; set; }
        public int Records { get; set; }
    }
}