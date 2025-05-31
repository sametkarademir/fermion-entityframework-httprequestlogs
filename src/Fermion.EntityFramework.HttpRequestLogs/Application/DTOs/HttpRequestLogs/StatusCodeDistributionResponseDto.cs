namespace Fermion.EntityFramework.HttpRequestLogs.Application.DTOs.HttpRequestLogs;

public class StatusCodeDistributionResponseDto
{
    public int StatusCode { get; set; }
    public int Count { get; set; }
}