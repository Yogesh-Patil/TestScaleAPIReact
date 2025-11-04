using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using PerfDemo.Models;
using Prometheus;

namespace PerfDemo.Services;

public class CachedDataService : IDataService
{
    private readonly SqlDataService _sql;
    private readonly IDistributedCache _cache;
    private static readonly Counter Hit = Metrics.CreateCounter("perf_demo_cache_hits_total", "cache hits");
    private static readonly Counter Miss = Metrics.CreateCounter("perf_demo_cache_misses_total", "cache misses");

    public CachedDataService(SqlDataService sql, IDistributedCache cache) { _sql = sql; _cache = cache; }

    public async Task<Product?> GetProductAsync(int id)
    {
        var key = $"product:{id}";
        var b = await _cache.GetAsync(key);
        if (b != null) { Hit.Inc(); return JsonSerializer.Deserialize<Product>(b); }
        Miss.Inc();
        var p = await _sql.GetProductAsync(id);
        if (p != null) await _cache.SetAsync(key, JsonSerializer.SerializeToUtf8Bytes(p), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });
        return p;
    }

    public async Task<List<Product>> GetProductsPageAsync(int page, int pageSize)
    {
        var key = $"products:page:{page}:size:{pageSize}";
        var b = await _cache.GetAsync(key);
        if (b != null) { Hit.Inc(); return JsonSerializer.Deserialize<List<Product>>(b)!; }
        Miss.Inc();
        var list = await _sql.GetProductsPageAsync(page, pageSize);
        await _cache.SetAsync(key, JsonSerializer.SerializeToUtf8Bytes(list), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });
        return list;
    }

    public async Task<Product> CreateProductAsync(string name, string payload)
    {
        var p = await _sql.CreateProductAsync(name, payload);
        // evict a few pages
        for (int i=1;i<=5;i++) await _cache.RemoveAsync($"products:page:{i}:size:20");
        return p;
    }

    public async Task<bool> UpdateProductAsync(int id, string name, string payload)
    {
        var ok = await _sql.UpdateProductAsync(id, name, payload);
        if (ok) { await _cache.RemoveAsync($"product:{id}"); for (int i=1;i<=5;i++) await _cache.RemoveAsync($"products:page:{i}:size:20"); }
        return ok;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var ok = await _sql.DeleteProductAsync(id);
        if (ok) { await _cache.RemoveAsync($"product:{id}"); for (int i=1;i<=5;i++) await _cache.RemoveAsync($"products:page:{i}:size:20"); }
        return ok;
    }
}
