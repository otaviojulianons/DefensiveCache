using CoreApp.DefensiveCache.Tests.Contracts;
using System.Threading.Tasks;

namespace CoreApp.DefensiveCache.Tests.Implementations
{
    public class ProductRepository : IProductRepository
    {
        public Product GetProduct(int id)
        {
            return new Product() { Id = 10, Name = "Notebook" };
        }

        public async Task<Product> GetProductAsync(int id)
        {
            return await Task.FromResult( new Product() { Id = 10, Name = "Notebook" });
        }
    }
}
