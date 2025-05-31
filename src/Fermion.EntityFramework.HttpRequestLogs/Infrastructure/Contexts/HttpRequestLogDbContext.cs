using Fermion.EntityFramework.HttpRequestLogs.Core.Entities;
using Fermion.EntityFramework.HttpRequestLogs.Infrastructure.EntityConfigurations;
using Fermion.EntityFramework.Shared.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Fermion.EntityFramework.HttpRequestLogs.Infrastructure.Contexts;

public class HttpRequestLogDbContext : DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DbSet<HttpRequestLog> HttpRequestLogs { get; set; }

    protected HttpRequestLogDbContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(HttpRequestLogConfiguration).Assembly);
    }

    public override int SaveChanges()
    {
        this.SetCreationTimestamps(_httpContextAccessor);
        this.SetModificationTimestamps(_httpContextAccessor);
        this.SetSoftDelete(_httpContextAccessor);
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        this.SetCreationTimestamps(_httpContextAccessor);
        this.SetModificationTimestamps(_httpContextAccessor);
        this.SetSoftDelete(_httpContextAccessor);
        return await base.SaveChangesAsync(cancellationToken);
    }
}