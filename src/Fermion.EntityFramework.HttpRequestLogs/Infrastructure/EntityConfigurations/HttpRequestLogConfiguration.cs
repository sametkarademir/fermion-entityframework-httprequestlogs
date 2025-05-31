using Fermion.EntityFramework.HttpRequestLogs.Core.Entities;
using Fermion.EntityFramework.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fermion.EntityFramework.HttpRequestLogs.Infrastructure.EntityConfigurations;

public class HttpRequestLogConfiguration : IEntityTypeConfiguration<HttpRequestLog>
{
    public void Configure(EntityTypeBuilder<HttpRequestLog> builder)
    {
        builder.ApplyGlobalEntityConfigurations();

        builder.ToTable("HttpRequestLogs");
        builder.HasIndex(item => item.HttpMethod);
        builder.HasIndex(item => item.RequestPath);
        builder.HasIndex(item => item.ClientIp);
        builder.HasIndex(item => item.DeviceFamily);
        builder.HasIndex(item => item.DeviceModel);
        builder.HasIndex(item => item.OsFamily);
        builder.HasIndex(item => item.OsVersion);
        builder.HasIndex(item => item.BrowserFamily);
        builder.HasIndex(item => item.BrowserVersion);
        builder.HasIndex(item => item.ControllerName);
        builder.HasIndex(item => item.ActionName);

        builder.Property(item => item.HttpMethod).HasMaxLength(10).IsRequired(false);
        builder.Property(item => item.RequestPath).HasMaxLength(1000).IsRequired(false);
        builder.Property(item => item.QueryString).IsRequired(false);
        builder.Property(item => item.RequestBody).IsRequired(false);
        builder.Property(item => item.RequestHeaders).IsRequired(false);

        builder.Property(item => item.StatusCode).IsRequired(false);

        builder.Property(item => item.RequestTime).IsRequired();
        builder.Property(item => item.ResponseTime).IsRequired();
        builder.Property(item => item.DurationMs).IsRequired(false);

        builder.Property(item => item.ClientIp).HasMaxLength(50).IsRequired(false);
        builder.Property(item => item.UserAgent).HasMaxLength(2000).IsRequired(false);

        builder.Property(item => item.DeviceFamily).HasMaxLength(100).IsRequired(false);
        builder.Property(item => item.DeviceModel).HasMaxLength(100).IsRequired(false);
        builder.Property(item => item.OsFamily).HasMaxLength(100).IsRequired(false);
        builder.Property(item => item.OsVersion).HasMaxLength(100).IsRequired(false);
        builder.Property(item => item.BrowserFamily).HasMaxLength(100).IsRequired(false);
        builder.Property(item => item.BrowserVersion).HasMaxLength(100).IsRequired(false);

        builder.Property(item => item.ControllerName).HasMaxLength(500).IsRequired(false);
        builder.Property(item => item.ActionName).HasMaxLength(500).IsRequired(false);
    }
}