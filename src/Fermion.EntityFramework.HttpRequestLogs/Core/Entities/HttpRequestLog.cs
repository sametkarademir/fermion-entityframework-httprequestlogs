using Fermion.Domain.Shared.Interfaces;
using Fermion.Domain.Shared.Auditing;
using Fermion.Domain.Shared.Filters;

namespace Fermion.EntityFramework.HttpRequestLogs.Core.Entities;

[ExcludeFromProcessing]
public class HttpRequestLog : CreationAuditedEntity<Guid>, IEntitySnapshotId, IEntitySessionId, IEntityCorrelationId
{
    // Request Information
    public string? HttpMethod { get; set; }
    public string? RequestPath { get; set; }
    public string? QueryString { get; set; }
    public string? RequestBody { get; set; }
    public string? RequestHeaders { get; set; }

    // Response Information
    public int? StatusCode { get; set; }

    // Performance Metrics
    public DateTime RequestTime { get; set; }
    public DateTime ResponseTime { get; set; }
    public long? DurationMs { get; set; }

    // Client Information
    public string? ClientIp { get; set; }
    public string? UserAgent { get; set; }

    // Device Information (parsed from UserAgent)
    public string? DeviceFamily { get; set; }
    public string? DeviceModel { get; set; }
    public string? OsFamily { get; set; }
    public string? OsVersion { get; set; }
    public string? BrowserFamily { get; set; }
    public string? BrowserVersion { get; set; }

    // API Information
    public string? ControllerName { get; set; }
    public string? ActionName { get; set; }

    public Guid? SnapshotId { get; set; }
    public Guid? SessionId { get; set; }
    public Guid? CorrelationId { get; set; }
}