namespace Fermion.EntityFramework.HttpRequestLogs.Application.DTOs.HttpRequestLogs;

public class ErrorRateOverTimeResponseDto
{
    public DateTime DateTime { get; set; }
    public int ErrorRate { get; set; }
    public int TotalCount { get; set; }
}