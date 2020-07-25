using CoreApp.DefensiveCache.Configuration;
using CoreApp.DefensiveCache.Serializers;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CoreApp.DefensiveCache.Proxy
{
    public class DefensiveCacheProxy<T> : DispatchProxy
    {

        public T Decorated { get; set; }

        public ILogger Logger { get; set; }

        public ICacheSerializer CacheFormatter { get; set; }


        public InterfaceCacheConfiguration RepositoryInterface { get; set; }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            try
            {
                var methodMatched = RepositoryInterface?.Methods.FirstOrDefault(x => x.Name == targetMethod.Name);
                if (methodMatched == null)
                    return targetMethod.Invoke(Decorated, args);

                return GetOrUpdateDynamic(targetMethod, args, methodMatched);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "CacheRepository reading fail.");
                return targetMethod.Invoke(Decorated, args);
            }
        }

        private dynamic GetOrUpdateDynamic(MethodInfo targetMethod, object[] args, MethodCacheConfiguration methodCacheConfiguration)
        {
            Type returnType = targetMethod.ReturnType;
            bool async = returnType.BaseType == typeof(Task);
            Type realReturnType = async ? returnType.GenericTypeArguments[0] : returnType;
            Type typeMethodCache = typeof(MethodCacheProxy<>).MakeGenericType(realReturnType);

            dynamic methodCache = Activator.CreateInstance(typeMethodCache, targetMethod, Decorated, args);
            methodCache.CacheFormatter = CacheFormatter;
            methodCache.CacheConfiguration = methodCacheConfiguration;
            return async ? methodCache.GetValueAsync() : methodCache.GetValue();
        }
    }
}
