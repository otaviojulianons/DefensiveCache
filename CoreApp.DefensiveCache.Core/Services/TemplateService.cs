using CoreApp.DefensiveCache.Configuration.Core;
using CoreApp.DefensiveCache.Extensions;
using CoreApp.DefensiveCache.Templates.Core;
using Mustache;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CoreApp.DefensiveCache.Services.Core
{
    public static class TemplateService
    {

        public static string GenerateCacheCodeFromType(this Type type, InterfaceCacheConfiguration cacheConfiguration)
        {
            var cacheTemplate = type.GetCacheTemplateFromType(cacheConfiguration);
            return GenerateCacheCode(cacheTemplate);
        }


        public static CacheTemplate GetCacheTemplateFromType(this Type type, InterfaceCacheConfiguration cacheConfiguration)
        {
            if (!type.IsInterface)
                throw new Exception("Only service interfaces are allowed for cache code generation.");

            var cacheTemplate = new CacheTemplate(type);
            foreach (var method in type.GetAllMethods())
            {
                var methodConfiguration = cacheConfiguration.Methods.FirstOrDefault(x => x.Name == method.Name);
                var cacheMethod = new CacheMethodTemplate(method, methodConfiguration);
                cacheTemplate.AddMethod(cacheMethod);
            };
            foreach (var property in type.GetProperties())
                cacheTemplate.AddProperty(property);

            var methodNamesCacheTemplate = cacheTemplate.Methods.Select(m => m.Name);
            var methodNamesCacheConfiguration = cacheConfiguration.Methods.Select(m => m.Name);
            var methodsNotMatched = methodNamesCacheConfiguration?.Where(x => !methodNamesCacheTemplate?.Contains(x) ?? false);
            if (methodsNotMatched?.Any() ?? false)
            {
                var methodName = methodsNotMatched.FirstOrDefault();
                throw new Exception($"Method '{methodName}' not found in interface {type.Name}.");
            }
               

            return cacheTemplate;
        }


        public static string GenerateCacheCode(CacheTemplate cacheTemplate)
        {
            var template = GetCacheTemplate();
            FormatCompiler compiler = new FormatCompiler() { RemoveNewLines = false };
            Generator generator = compiler.Compile(template);
            return generator.Render(cacheTemplate);
        }

        private static string GetCacheTemplate()
        {
            var assembly = Assembly.GetAssembly(typeof(TemplateService));
            var resourceName = "CoreApp.DefensiveCache.Core.Templates._CacheProxyTemplate.mustache";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
