using CoreApp.DefensiveCache.Configuration;
using CoreApp.DefensiveCache.Extensions;
using CoreApp.DefensiveCache.Formatters;
using CoreApp.DefensiveCache.Services;
using CoreApp.DefensiveCache.Templates;
using CoreApp.DefensiveCache.Tests.Contracts;
using CoreApp.DefensiveCache.Tests.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Linq;
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
            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton<ICacheFormatter, JsonNetCacheFormatter>();
            services.AddDistributedMemoryCache();

            _serviceProvider = services.BuildServiceProvider();
        }
        private CacheTemplate GetCacheTemplate()
        {
            
            var model = new CacheTemplate();
            model.Name = "ProductRepositoryDynamicCache";
            model.InterfaceName = "UnitTests.Mocks.IProductRepository";
            model.AddMethod(new CacheMethodTemplate()
            {
                ReturnType = "UnitTests.Mocks.Product",
                Name = "GetProduct",
                ParametersDeclarations = "System.Int32 id",
                ParametersNames = "id",
                Enabled = true,
                CacheKeyTemplate = "product_{id}",
                CacheExpirationSeconds = 60
            });
            model.AddMethod(new CacheMethodTemplate()
            {
                ReturnType = "System.Threading.Tasks.Task<UnitTests.Mocks.Product>",
                Name = "GetProductAsync",
                ParametersDeclarations = "System.Int32 id",
                ParametersNames = "id",
                Enabled = false,
                CacheKeyTemplate = "product_async{id}",
                CacheExpirationSeconds = 60
            });
            return model;
        }

        [Fact]
        public void GenerateClassCodeByTemplate()
        {
            var model = GetCacheTemplate();
            var classFile = TemplateService.Generate(model);
            Assert.Contains("public class ProductRepositoryDynamicCache", classFile);
            Assert.Contains("public UnitTests.Mocks.Product GetProduct(System.Int32 id)", classFile);
        }

        [Fact]
        public void GenerateClassCodeByType()
        {
            var model = ReflectionService.GetTemplateModel(typeof(IProductRepository), new InterfaceCacheConfiguration());
            var classFile = TemplateService.Generate(model);
            Assert.Contains("public class IProductRepositoryDynamicCache", classFile);
            Assert.Contains("public UnitTests.Mocks.Product GetProduct(System.Int32 id)", classFile);
        }

        [Fact]
        public void CompileClassCodeFromString()
        {
            var model = GetCacheTemplate();
            var classFile = TemplateService.Generate(model);
            var assembly = CompilerService.GenerateAssemblyFromCode(typeof(IProductRepository).Assembly , classFile);
            var type = assembly.GetTypes().FirstOrDefault();
            Assert.Contains(model.Name, assembly.GetTypes().Select(x => x.Name));
        }


        [Fact]
        public async Task DecorateClassGenerated()
        {
            var cacheFormatter = _serviceProvider.GetService<ICacheFormatter>();
            var model = GetCacheTemplate();
            var classFile = TemplateService.Generate(model);
            var assembly = CompilerService.GenerateAssemblyFromCode(typeof(IProductRepository).Assembly, classFile);
            var type = assembly.GetTypes().FirstOrDefault();

            var concreteRepository = new ProductRepository();
            var cacheRepository = ReflectionExtensions
                .CreateInstanceDecorated<IProductRepository>(type, concreteRepository, cacheFormatter);
            
            var product1 = cacheRepository.GetProduct(1);
            product1 = cacheRepository.GetProduct(1);

            var product2 = await cacheRepository.GetProductAsync(1);
            product2 = await cacheRepository.GetProductAsync(1);

            Assert.Equal(10, product1.Id);
            Assert.Equal(10, product2.Id);
        }
    }
}
