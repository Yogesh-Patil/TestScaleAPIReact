using Microsoft.EntityFrameworkCore;
using PerfDemo.Models;

namespace PerfDemo.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }
    public DbSet<Product> Products => Set<Product>();
}
