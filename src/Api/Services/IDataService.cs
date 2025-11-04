using PerfDemo.Models;

namespace PerfDemo.Services;

public interface IDataService
{
    Task<Product?> GetProductAsync(int id);
    Task<List<Product>> GetProductsPageAsync(int page, int pageSize);
    Task<Product> CreateProductAsync(string name, string payload);
    Task<bool> UpdateProductAsync(int id, string name, string payload);
    Task<bool> DeleteProductAsync(int id);
}
