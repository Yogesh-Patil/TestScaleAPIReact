using Microsoft.EntityFrameworkCore;
using PerfDemo.Data;
using PerfDemo.Services;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// Config
var primary = builder.Configuration["ConnectionStrings:Primary"] ?? Environment.GetEnvironmentVariable("ConnectionStrings__Primary");
var replica = builder.Configuration["ConnectionStrings:ReadReplica"] ?? Environment.GetEnvironmentVariable("ConnectionStrings__ReadReplica");
var redis   = builder.Configuration["Redis:Configuration"] ?? Environment.GetEnvironmentVariable("Redis__Configuration");

// DbFactory registration
builder.Services.AddSingleton(new DbContextFactoryOptions { PrimaryConnection = primary!, ReadReplicaConnection = replica ?? primary! });
builder.Services.AddScoped<DbContextFactory>();

// Caching
if (!string.IsNullOrEmpty(redis))
    builder.Services.AddStackExchangeRedisCache(opt => opt.Configuration = redis);
else
    builder.Services.AddDistributedMemoryCache();

// Services
builder.Services.AddScoped<SqlDataService>();
builder.Services.AddScoped<CachedDataService>();
builder.Services.AddScoped<IDataService>(sp => sp.GetRequiredService<CachedDataService>());

// EF Core design-time factory registration for migrations (optional)
builder.Services.AddDbContextFactory<AppDbContext>(opt => opt.UseSqlServer(primary));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseCors("AllowAll");
app.UseRouting();
app.UseHttpMetrics();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapMetrics();
});

// Ensure DB created
using (var scope = app.Services.CreateScope())
{
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
    var ctx = factory.CreateDbContext();
    ctx.Database.EnsureCreated();
}

app.Run();
