using CoreApp.DefensiveCache.Configuration;
using CoreApp.DefensiveCache.Extensions;
using CoreApp.DefensiveCache.Templates;
using Stubble.Core;
using Stubble.Core.Builders;
using Stubble.Core.Settings;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CoreApp.DefensiveCache.Services
{
    public static class TemplateService
    {
        private static RenderSettings _renderSettings;

        public static string GenerateCacheCodeFromType(this Type type, InterfaceCacheConfiguration cacheConfiguration)
        {
            var cacheTemplate = type.GetCacheTemplateFromType(cacheConfiguration);
            return GenerateCacheCode(cacheTemplate);
        }


        public static CacheTemplate GetCacheTemplateFromType(this Type type, InterfaceCacheConfiguration cacheConfiguration)
        {
            if (!type.IsInterface)
                throw new Exception("Only service interfaces are allowed for cache code generation.");

            var cacheTemplate = new CacheTemplate();
            cacheTemplate.Name = type.Name + "DynamicCache";
            cacheTemplate.InterfaceName = type.FullName;

            foreach (var method in type.GetAllMethods())
            {
                var methodConfiguration = cacheConfiguration.Methods.FirstOrDefault(x => x.Name == method.Name);
                var cacheMethod = new CacheMethodTemplate(method, methodConfiguration);
                cacheTemplate.AddMethod(cacheMethod);
            };

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
            if(_renderSettings == null)
            {
                _renderSettings = RenderSettings.GetDefaultRenderSettings();
                _renderSettings.SkipHtmlEncoding = true;
            }
            StubbleVisitorRenderer _stubbleTemplate = new StubbleBuilder().Build();
            return _stubbleTemplate.Render(GetCacheTemplate(), cacheTemplate, _renderSettings);
        }

        private static string GetCacheTemplate()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "CoreApp.DefensiveCache.Templates._CacheProxyTemplate.mustache";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
