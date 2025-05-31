namespace Fermion.EntityFramework.HttpRequestLogs.Application.DTOs.HttpRequestLogs;

public class EndpointPerformanceResponseDto
{
    public string Path { get; set; } = string.Empty;
    public double AverageResponseTime { get; set; }
    public int RequestCount { get; set; }
    public double ErrorRate { get; set; }
}