using System.Threading.Tasks;

namespace CoreApp.DefensiveCache.Tests.Contracts
{
    public interface IProductRepository
    {
        Product GetProduct(int id);
        Task<Product> GetProductAsync(int id);
    }
}
