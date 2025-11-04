using Microsoft.EntityFrameworkCore;

namespace PerfDemo.Data;

public class DbContextFactoryOptions
{
    public string PrimaryConnection { get; set; } = null!;
    public string ReadReplicaConnection { get; set; } = null!;
}

public class DbContextFactory
{
    private readonly DbContextFactoryOptions _opts;
    public DbContextFactory(DbContextFactoryOptions opts) => _opts = opts;
    public AppDbContext CreatePrimary()
    {
        var builder = new DbContextOptionsBuilder<AppDbContext>();
        builder.UseSqlServer(_opts.PrimaryConnection);
        return new AppDbContext(builder.Options);
    }
    public AppDbContext CreateReplica()
    {
        var builder = new DbContextOptionsBuilder<AppDbContext>();
        builder.UseSqlServer(_opts.ReadReplicaConnection ?? _opts.PrimaryConnection);
        return new AppDbContext(builder.Options);
    }
}
