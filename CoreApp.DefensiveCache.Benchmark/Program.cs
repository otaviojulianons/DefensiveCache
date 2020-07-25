using BenchmarkDotNet.Running;

namespace CoreApp.DefensiveCache.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length > 0 && args[0] == "run")
            {
                var banchmark = new Benchmark();
                banchmark.ProcessDefensiveCacheGenerated().Wait();
                banchmark.ProcessDefensiveCacheDynamicProxy().Wait();
                banchmark.ProcessDefensiveCacheImplemented().Wait();
            }
            else
                BenchmarkRunner.Run<Benchmark>();
        }
    }
}
