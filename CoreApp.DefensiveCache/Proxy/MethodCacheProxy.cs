using CoreApp.DefensiveCache.Configuration;
using CoreApp.DefensiveCache.Extensions;
using CoreApp.DefensiveCache.Formatters;
using Microsoft.Extensions.Caching.Distributed;
using Stubble.Core;
using Stubble.Core.Builders;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Threading.Tasks;

namespace CoreApp.DefensiveCache.Proxy
{
    public class MethodCacheProxy<T>
    {
        private StubbleVisitorRenderer _stubbleTemplate = new StubbleBuilder().Build();
        private MethodInfo _targetMethod;
        private object _object;
        private object[] _args;

        public MethodCacheProxy(MethodInfo targetMethod, object @object, object[] args)
        {
            _targetMethod = targetMethod;
            _object = @object;
            _args = args;
        }

        public ICacheFormatter CacheFormatter { get; set; }

        public MethodCacheConfiguration CacheConfiguration { get; set; }

        private T GetValueConcrete() => (T)_targetMethod.Invoke(_object, _args);
        private Task<T> GetValueConcreteAsync() => (Task<T>)_targetMethod.Invoke(_object, _args);

        private string GetCacheKey()
        {
            var methodProperties = new ExpandoObject();
            var expandoObjCollection = (ICollection<KeyValuePair<string, object>>)methodProperties;
            var parameters = _targetMethod.GetParameters();
            for (int parameterIndex = 0; parameterIndex < parameters.Length; parameterIndex++)
            {
                var parameterName = parameters[parameterIndex].Name;
                var value = _args[parameterIndex];
                expandoObjCollection.Add(new KeyValuePair<string, object>(parameterName, value));
            }

            return _stubbleTemplate.Render(CacheConfiguration.KeyTemplate, methodProperties);
        }

        public T GetValue()
        {
            var cacheKey = GetCacheKey();
            var cacheValue = CacheFormatter.Get<T>(cacheKey);
            if (Equals(cacheValue, default(T)))
            {
                cacheValue = GetValueConcrete();
                CacheFormatter.SetAsync(cacheKey, cacheValue, CacheConfiguration.ExpirationSeconds);
            }
            return cacheValue;
        }

        public async Task<T> GetValueAsync()
        {
            var cacheKey = GetCacheKey();
            var cacheValue = await CacheFormatter.GetAsync<T>(cacheKey);
            if (Equals(cacheValue, default(T)))
            {
                cacheValue = await GetValueConcreteAsync();
                await CacheFormatter.SetAsync(cacheKey, cacheValue, CacheConfiguration.ExpirationSeconds);
            }
            return cacheValue;
        }

    }
}
