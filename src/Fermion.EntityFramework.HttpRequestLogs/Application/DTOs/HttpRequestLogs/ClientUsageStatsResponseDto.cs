namespace Fermion.EntityFramework.HttpRequestLogs.Application.DTOs.HttpRequestLogs;

public class ClientUsageStatsResponseDto
{
    public Dictionary<string, int> BrowserFamilies { get; set; } = new();
    public Dictionary<string, int> OsFamilies { get; set; } = new();
    public Dictionary<string, int> DeviceFamilies { get; set; } = new();
    public Dictionary<string, int> BrowserVersions { get; set; } = new();
    public Dictionary<string, int> OsVersions { get; set; } = new();
    public Dictionary<string, int> DeviceTypeDistribution { get; set; } = new();
    public int TotalRequests { get; set; }
}