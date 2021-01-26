using CoreApp.DefensiveCache.Configuration;
using CoreApp.DefensiveCache.Extensions;
using CoreApp.DefensiveCache.Serializers;
using CoreApp.DefensiveCache.Services;
using CoreApp.DefensiveCache.Templates;
using CoreApp.DefensiveCache.Tests.Contracts;
using CoreApp.DefensiveCache.Tests.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace CoreApp.DefensiveCache.Tests
{
    public class UnitTests
    {
        private ServiceProvider _serviceProvider;

        public UnitTests()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();

            var services = new ServiceCollection();
            services.AddLogging();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton<ICacheSerializer, JsonNetCacheSerializer>();
            services.AddDistributedMemoryCache();

            _serviceProvider = services.BuildServiceProvider();
        }

        private CacheTemplate GetCacheTemplateFromProductRepository()
        {
            var type = typeof(IProductRepository);
            var model = new CacheTemplate(type);

            var getProduct = type.GetMethod(nameof(IProductRepository.GetProduct));
            var getProductAsync = type.GetMethod(nameof(IProductRepository.GetProductAsync));

            var getProductConfig = new MethodCacheConfiguration()
            {
                Name = getProduct.Name,
                ExpirationSeconds = 60,
                KeyTemplate = "product_{id}"
            };
            var getProductAsyncConfig = new MethodCacheConfiguration()
            {
                Name = getProduct.Name,
                ExpirationSeconds = 60,
                KeyTemplate = "product_async_{id}"
            };

            model.AddMethod(new CacheMethodTemplate(getProduct, getProductConfig));
            model.AddMethod(new CacheMethodTemplate(getProductAsync, getProductAsyncConfig));

            return model;
        }

        private CacheTemplate GetCacheTemplateFromGroupRepository()
        {
            var type = typeof(IGroupRepository);
            var model = new CacheTemplate(type);

            var getGroup = type.GetMethod(nameof(IGroupRepository.GetGroup));
            var getGroupConfig = new MethodCacheConfiguration()
            {
                Name = getGroup.Name,
                ExpirationSeconds = 60,
                KeyTemplate = "group_{id}"
            };

            model.AddMethod(new CacheMethodTemplate(getGroup, getGroupConfig));
            return model;
        }

        [Fact]
        public void GenerateClassCodeByTemplate()
        {
            var model = GetCacheTemplateFromProductRepository();
            var classFile = TemplateService.GenerateCacheCode(model);
            Assert.Contains("public class IProductRepositoryDynamicCache", classFile);
            Assert.Contains("public CoreApp.DefensiveCache.Tests.Contracts.Product GetProduct(System.Int32 id)", classFile);
        }

        [Fact]
        public void GenerateClassCodeByType()
        {
            var cacheCode = typeof(IProductRepository).GenerateCacheCodeFromType(new InterfaceCacheConfiguration());
            Assert.Contains("public class IProductRepositoryDynamicCache", cacheCode);
            Assert.Contains("public CoreApp.DefensiveCache.Tests.Contracts.Product GetProduct(System.Int32 id)", cacheCode);
        }

        [Fact]
        public void CompileAssemblyFromTemplate()
        {
            var model = GetCacheTemplateFromProductRepository();
            var classFile = TemplateService.GenerateCacheCode(model);
            var assembly = CompilerService.GenerateAssemblyFromCode(typeof(IProductRepository).Assembly, model.Name, classFile);
            var type = assembly.GetTypes().FirstOrDefault();
            Assert.Contains(model.Name, assembly.GetTypes().Select(x => x.Name));
        }


        [Fact]
        public async Task DecorateServiceWithDynamicAssembly()
        {
            var configuration = _serviceProvider.GetService<IConfiguration>();
            var cacheFormatter = _serviceProvider.GetService<ICacheSerializer>();
            var model = GetCacheTemplateFromProductRepository();
            var classFile = TemplateService.GenerateCacheCode(model);
            var assembly = CompilerService.GenerateAssemblyFromCode(typeof(IProductRepository).Assembly, model.Name, classFile);
            var typeGenerated = assembly.GetTypes().FirstOrDefault();

            var concreteRepository = new ProductRepository();
            var cacheRepository = (IProductRepository)Activator.CreateInstance(typeGenerated, concreteRepository, configuration, cacheFormatter);

            var product1 = cacheRepository.GetProduct(1);
            product1 = cacheRepository.GetProduct(1);

            var product2 = await cacheRepository.GetProductAsync(1);
            product2 = await cacheRepository.GetProductAsync(1);

            Assert.Equal(10, product1.Id);
            Assert.Equal(10, product2.Id);
        }

        [Fact]
        public void GetCacheTemplateFromReflectionType()
        {
            var reflectionType = typeof(IProductRepository);
            var cacheConfiguration = new InterfaceCacheConfiguration();
            var template = reflectionType.GetCacheTemplateFromReflectionType(cacheConfiguration);

            Assert.Equal(reflectionType.FullName, template.InterfaceName);
        }

        [Fact]
        public void CompileMultipleAssemblies()
        {
            var model = GetCacheTemplateFromProductRepository();
            var classFile = TemplateService.GenerateCacheCode(model);
            var assembly1 = CompilerService.GenerateAssemblyFromCode(typeof(IProductRepository).Assembly, model.Name, classFile);
            var assembly2 = CompilerService.GenerateAssemblyFromCode(typeof(IProductRepository).Assembly, model.Name, classFile);
            Assert.NotEqual(assembly1.GetHashCode(), assembly2.GetHashCode());
        }

    }
}
