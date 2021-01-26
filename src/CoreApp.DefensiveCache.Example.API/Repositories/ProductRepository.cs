using CoreApp.DefensiveCache.Tests.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace CoreApp.DefensiveCache.Tests.Implementations
{
    public class ProductRepository : IProductRepository
    {
        public Product GetProduct(int id)
        {
            Thread.Sleep(2000);
            return new Product() { Id = 10, Name = "Notebook", GroupId = 1 };
        }

        public async Task<Product> GetProductAsync(int id)
        {
            await Task.Delay(2000);
            return await Task.FromResult( new Product() { Id = 20, Name = "Smartphone X", GroupId = 2 });
        }
    }
}
