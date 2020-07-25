using CoreApp.DefensiveCache.Configuration;
using CoreApp.DefensiveCache.Formatters;
using CoreApp.DefensiveCache.Proxy;
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
        public static void DecorateWithCacheFromConfiguration(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var conf = serviceProvider.GetService<IConfiguration>();
            var cacheRepositories = conf.GetSection("Cache:Repositories").Get<IEnumerable<InterfaceCacheConfiguration>>();

            foreach (var repository in cacheRepositories)
                services.DecorateWithCacheConfiguration(repository);
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
            var templateModel = ReflectionService.GetTemplateModel(typeService, cacheConfiguration);
            var classCode = TemplateService.Generate(templateModel);
            var assembly = CompilerService.GenerateAssemblyFromCode(typeService.Assembly, classCode);
            var typeCache = assembly.GetTypes().FirstOrDefault();
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
            var cacheFormatter = services.GetService<ICacheFormatter>();

            proxy.Decorated = decoratedService;
            proxy.Logger = logger;
            proxy.CacheFormatter = cacheFormatter;
            proxy.RepositoryInterface = repositoryInterface;
            return proxy;
        }
    }
}
