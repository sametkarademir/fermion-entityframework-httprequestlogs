using System.Reflection;
using Fermion.EntityFramework.HttpRequestLogs.Infrastructure.EntityConfigurations;
using Fermion.EntityFramework.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using WebApi.Entities;

namespace WebApi.Contexts;

public class ApplicationDbContext : DbContext
{
    public DbSet<Category> Categories { get; set; }

    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
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