using Microsoft.EntityFrameworkCore;
using PerfDemo.Data;
using PerfDemo.Models;
using Prometheus;

namespace PerfDemo.Services;

public class SqlDataService
{
    private readonly DbContextFactory _dbFactory;
    private static readonly Histogram DbHistogram = Metrics.CreateHistogram("perf_demo_db_query_duration_seconds", "DB query duration");

    public SqlDataService(DbContextFactory dbFactory) => _dbFactory = dbFactory;

    public async Task<Product?> GetProductAsync(int id)
    {
        using var ctx = _dbFactory.CreateReplica();
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var p = await ctx.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        sw.Stop();
        DbHistogram.Observe(sw.Elapsed.TotalSeconds);
        return p;
    }

    public async Task<List<Product>> GetProductsPageAsync(int page, int pageSize)
    {
        using var ctx = _dbFactory.CreateReplica();
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var list = await ctx.Products.AsNoTracking().OrderBy(p => p.Id).Skip((page-1)*pageSize).Take(pageSize).ToListAsync();
        sw.Stop();
        DbHistogram.Observe(sw.Elapsed.TotalSeconds);
        return list;
    }

    public async Task<Product> CreateProductAsync(string name, string payload)
    {
        using var ctx = _dbFactory.CreatePrimary();
        var p = new Product { Name = name, Payload = payload, CreatedAt = DateTime.UtcNow };
        ctx.Products.Add(p);
        await ctx.SaveChangesAsync();
        return p;
    }

    public async Task<bool> UpdateProductAsync(int id, string name, string payload)
    {
        using var ctx = _dbFactory.CreatePrimary();
        var p = await ctx.Products.FindAsync(id);
        if (p == null) return false;
        p.Name = name; p.Payload = payload;
        await ctx.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        using var ctx = _dbFactory.CreatePrimary();
        var p = await ctx.Products.FindAsync(id);
        if (p == null) return false;
        ctx.Products.Remove(p);
        await ctx.SaveChangesAsync();
        return true;
    }
}
