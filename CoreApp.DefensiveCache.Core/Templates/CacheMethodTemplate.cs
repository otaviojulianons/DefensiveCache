using CoreApp.DefensiveCache.Configuration;
using CoreApp.DefensiveCache.Extensions;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CoreApp.DefensiveCache.Templates.Core
{
    public class CacheMethodTemplate
    {
        public CacheMethodTemplate(MethodInfo methodInfo = null, Configuration.Core.MethodCacheConfiguration methodConfiguration = null)
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

        public bool Async { get; private set; }
        public string ReturnType { get; private set; }
        public string ReturnTypeBase { get; private set; }
        public bool Enabled { get; private set; }
        public string Name { get; private set; }
        public string ParametersDeclarations { get; private set; }
        public string ParametersNames { get; private set; }
        public string CacheKeyTemplate { get; private set; }
        public double CacheExpirationSeconds { get; private set; }
        public bool ReturnValue => !string.IsNullOrEmpty(ReturnType);

    }
}
