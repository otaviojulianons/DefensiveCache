using CoreApp.DefensiveCache.Configuration;
using CoreApp.DefensiveCache.Extensions;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CoreApp.DefensiveCache.Templates
{
    public class CacheMethodTemplate
    {
        public CacheMethodTemplate(MethodInfo methodInfo = null, MethodCacheConfiguration methodConfiguration = null)
        {
            if (methodInfo == null)
                return;

            var returnType = methodInfo.ReturnType;
            Async = returnType.BaseType == typeof(Task);
            ReturnType = returnType.GetDeclarationType();
            ReturnTypeBase = Async ? returnType.GenericTypeArguments[0].GetDeclarationType() : ReturnType;
            Name = methodInfo.Name;
            ParametersNames = string.Join(",", methodInfo.GetParameters().Select(x => x.Name));
            ParametersDeclarations = string.Join(",", methodInfo.GetParameters().Select(x => x.GetDeclaration()));
            Enabled = methodConfiguration != null;
            CacheExpirationSeconds = methodConfiguration?.ExpirationSeconds ?? 0;
            CacheKeyTemplate = methodConfiguration?.KeyTemplate;
        }

        public bool Async { get; set; }
        public string ReturnType { get; set; }
        public string ReturnTypeBase { get; set; }
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public string ParametersDeclarations { get; set; }
        public string ParametersNames { get; set; }
        public string CacheKeyTemplate { get; set; }
        public double CacheExpirationSeconds { get; set; }
        public bool ReturnValue => !string.IsNullOrEmpty(ReturnType);

    }
}
