namespace Fermion.EntityFramework.HttpRequestLogs.Application.DTOs.HttpRequestLogs;

public class ResponseTimeStatsResponseDto
{
    public string Path { get; set; } = string.Empty;
    public int RequestCount { get; set; }
    public long MinResponseTime { get; set; }
    public long MaxResponseTime { get; set; }
    public double AverageResponseTime { get; set; }
    public long MedianResponseTime { get; set; }
    public long Percentile90 { get; set; }
    public long Percentile95 { get; set; }
    public long Percentile99 { get; set; }
}