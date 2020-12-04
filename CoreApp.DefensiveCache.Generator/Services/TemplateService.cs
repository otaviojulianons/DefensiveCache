using CoreApp.DefensiveCache.Configuration.Core;
using CoreApp.DefensiveCache.Core.Interfaces;
using CoreApp.DefensiveCache.Extensions.Core;
using CoreApp.DefensiveCache.Templates.Core;
using Stubble.Core;
using Stubble.Core.Builders;
using Stubble.Core.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CoreApp.DefensiveCache.Services.Core
{
    public static class TemplateService
    {
        private static RenderSettings _renderSettings;

        public static string GenerateCacheCodeFromType(this Type type, InterfaceCacheConfiguration cacheConfiguration)
        {
            var cacheTemplate = type.GetCacheTemplateFromReflectionType(cacheConfiguration);
            return GenerateCacheCode(cacheTemplate);
        }

        public static CacheTemplate GetCacheTemplateFromReflectionType(this Type type, InterfaceCacheConfiguration cacheConfiguration)
        {
            if (type == null)
                throw new Exception("Service type is no defined.");

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

        public static IEnumerable<CacheTemplate> GetCacheTemplatesFromAssembly(string assemplyPath)
        {
            if (!File.Exists(assemplyPath))
                yield break;
            var assembly = Assembly.LoadFrom(assemplyPath);
            var cacheServiceTypes = assembly.GetCacheServiceMappers().SelectMany(GetCacheServiceMappers);
            foreach (var cacheServiceType in cacheServiceTypes)
                yield return cacheServiceType.Type.GetCacheTemplateFromReflectionType(cacheServiceType.Configuration);
        }

        private static IEnumerable<Type> GetCacheServiceMappers(this Assembly assembly) =>
            assembly.GetTypes().Where(x => x.IsClass && (x.GetInterfaces()?.Contains(typeof(ICacheServiceMapper)) ?? false));

        private static IEnumerable<(Type Type, InterfaceCacheConfiguration Configuration)> GetCacheServiceMappers(Type cacheServiceMapperType)
        {
            var cacheServiceMapper = (ICacheServiceMapper)Activator.CreateInstance(cacheServiceMapperType);
            var cacheConfiguration = new CacheConfiguration();
            cacheServiceMapper.Map(cacheConfiguration);
            foreach (var service in cacheConfiguration.Services)
                if (service is InterfaceCacheConfigurationTyped configurationTyped)
                    yield return (configurationTyped.ServiceType, configurationTyped);
        }

        public static string GenerateCacheCode(params CacheTemplate[] cacheTemplates)
        {
            if (_renderSettings == null)
            {
                _renderSettings = RenderSettings.GetDefaultRenderSettings();
                _renderSettings.SkipHtmlEncoding = true;
            }
            StubbleVisitorRenderer _stubbleTemplate = new StubbleBuilder().Build();
            var cacheTemplate = new { Services = cacheTemplates };
            return _stubbleTemplate.Render(GetCacheTemplate(), cacheTemplate, _renderSettings);
        }

        private static string GetCacheTemplate()
        {
            var assembly = Assembly.GetAssembly(typeof(TemplateService));
            var resourceName = "CoreApp.DefensiveCache.Generator.Templates._GenerateFile.mustache";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
