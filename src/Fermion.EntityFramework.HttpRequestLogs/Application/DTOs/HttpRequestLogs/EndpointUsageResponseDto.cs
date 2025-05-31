namespace Fermion.EntityFramework.HttpRequestLogs.Application.DTOs.HttpRequestLogs;

public class EndpointUsageResponseDto
{
    public string Path { get; set; } = string.Empty;
    public int RequestCount { get; set; }
    public double AverageResponseTime { get; set; }
}