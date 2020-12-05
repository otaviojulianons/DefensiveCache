using CoreApp.DefensiveCache.Configuration;
using CoreApp.DefensiveCache.Proxy;
using CoreApp.DefensiveCache.Serializers;
using CoreApp.DefensiveCache.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CoreApp.DefensiveCache.Extensions
{
    public static class DefensiveCacheExtensions
    {        
        public static void DecorateWithCacheGeneratedFromConfiguration(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var conf = serviceProvider.GetService<IConfiguration>();
            var dynamicServices = conf.GetSection("Cache:DynamicServices").GetChildren();
            foreach (var serviceConfiguration in dynamicServices)
            {
                var interfaceCacheConfiguration = new InterfaceCacheConfiguration(serviceConfiguration.Key);
                var methodsConfiguration = serviceConfiguration.GetChildren();
                foreach (var methodConfiguration in methodsConfiguration)
                {
                    var methodName = methodConfiguration.Key;
                    var methodCacheConfiguration = methodConfiguration.Get<MethodCacheConfiguration>();
                    interfaceCacheConfiguration
                        .AddMethod(methodName, methodCacheConfiguration.KeyTemplate, methodCacheConfiguration.ExpirationSeconds);
                }
                services.DecorateWithCacheConfiguration(interfaceCacheConfiguration);
            }
                
        }

        public static void DecorateWithCacheGenerated<T>(
            this IServiceCollection services,
            Action<InterfaceCacheConfigurationTyped<T>> configure)
        {
            var configuration = new InterfaceCacheConfigurationTyped<T>();
            configure(configuration);
            services.DecorateWithCacheConfiguration(configuration);
        }

        public static void DecorateWithCacheProxy<T>(
            this IServiceCollection services,
            Action<InterfaceCacheConfigurationTyped<T>> configure)
        {
            var configuration = new InterfaceCacheConfigurationTyped<T>();
            configure(configuration);
            services.DecorateWithCacheProxy(configuration);
        }

        public static void DecorateWithCacheConfiguration(this IServiceCollection services, InterfaceCacheConfiguration cacheConfiguration)
        {
            var serviceMatched = services.FirstOrDefault(x => x.ServiceType.Name == cacheConfiguration.Name);
            if (serviceMatched == null)
                return;

            var typeService = serviceMatched.ServiceType;
            var cacheTemplate = typeService.GetCacheTemplateFromReflectionType(cacheConfiguration);
            var cacheCode = TemplateService.GenerateCacheCode(cacheTemplate);
            var assemblyCache = CompilerService.GenerateAssemblyFromCode(typeService.Assembly, cacheTemplate.Name, cacheCode);
            var typeCache = assemblyCache.GetTypes().FirstOrDefault();
            services.Decorate(typeService, typeCache);
        }

        private static void DecorateWithCacheProxy(this IServiceCollection services, InterfaceCacheConfiguration repositoryInterface)
        {
            var serviceMatched = services.FirstOrDefault(x => x.ServiceType.Name == repositoryInterface.Name);
            if (serviceMatched == null)
                return;

            var typeService = serviceMatched.ServiceType;
            services.Decorate(typeService, (service, provider) =>
            {
                return CreateCacheProxy(provider, typeService, service, repositoryInterface);
            });
        }

        private static dynamic CreateCacheProxy(
            IServiceProvider services,
            Type typeService, 
            dynamic decoratedService,
            InterfaceCacheConfiguration repositoryInterface)
        {
            var nameCreateMethod = nameof(DispatchProxy.Create);
            var dispatchProxyMethod = typeof(DispatchProxy).GetMethod(nameCreateMethod);
            var cacheRepositoryProxy = typeof(DefensiveCacheProxy<>).MakeGenericType(typeService);
            dynamic proxy = dispatchProxyMethod.MakeGenericMethod(typeService, cacheRepositoryProxy).Invoke(null, null);
            
            var typeLogger = typeof(ILogger<>).MakeGenericType(typeService);
            var logger = (ILogger)services.GetService(typeLogger);
            var cacheSerializer = services.GetService<ICacheSerializer>();

            proxy.Decorated = decoratedService;
            proxy.Logger = logger;
            proxy.CacheSerializer = cacheSerializer;
            proxy.RepositoryInterface = repositoryInterface;
            return proxy;
        }
    }
}
