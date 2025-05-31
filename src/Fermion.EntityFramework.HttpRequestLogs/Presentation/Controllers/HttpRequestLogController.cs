using Fermion.EntityFramework.HttpRequestLogs.Application.DTOs.HttpRequestLogs;
using Fermion.EntityFramework.HttpRequestLogs.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fermion.EntityFramework.HttpRequestLogs.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HttpRequestLogController(
    IHttpRequestLogAppService httpRequestLogAppService)
    : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await httpRequestLogAppService.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpGet("pageable")]
    public async Task<ActionResult> GetPageableAndFilterAsync(
        [FromQuery] GetListHttpRequestLogRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var result = await httpRequestLogAppService.GetPageableAndFilterAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("cleanup")]
    public async Task<ActionResult> CleanupOldHttpRequestLogsAsync(
        [FromQuery] DateTime olderThan,
        CancellationToken cancellationToken = default)
    {
        var result = await httpRequestLogAppService.CleanupOldHttpRequestLogsAsync(olderThan, cancellationToken);
        return Ok(result);
    }

    [HttpGet("response-time-stats")]
    public async Task<ActionResult> GetResponseTimeStatsAsync(
        [FromQuery] ResponseTimeStatsRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var result = await httpRequestLogAppService.GetResponseTimeStatsAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("slowest-endpoints")]
    public async Task<ActionResult> GetSlowestEndpointsAsync(
        [FromQuery] EndpointPerformanceRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var result = await httpRequestLogAppService.GetSlowestEndpointsAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("most-frequent-endpoints")]
    public async Task<ActionResult> GetMostFrequentEndpointsAsync(
        [FromQuery] EndpointUsageRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var result = await httpRequestLogAppService.GetMostFrequentEndpointsAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("status-code-distribution")]
    public async Task<ActionResult> GetStatusCodeDistributionAsync(
        [FromQuery] StatusCodeDistributionRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var result = await httpRequestLogAppService.GetStatusCodeDistributionAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("request-rate")]
    public async Task<ActionResult> GetRequestRateOverTimeAsync(
        [FromQuery] RateOverTimeRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var result = await httpRequestLogAppService.GetRequestRateOverTimeAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("error-rate")]
    public async Task<ActionResult> GetErrorRateOverTimeAsync(
        [FromQuery] RateOverTimeRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var result = await httpRequestLogAppService.GetErrorRateOverTimeAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("client-usage-stats")]
    public async Task<ActionResult> GetClientUsageStatsAsync(
        [FromQuery] DateRangeRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var result = await httpRequestLogAppService.GetClientUsageStatsAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("count")]
    public async Task<ActionResult> GetTotalLogCountAsync(
        [FromQuery] DateRangeRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var result = await httpRequestLogAppService.GetTotalLogCountAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("export-csv")]
    public async Task<ActionResult> ExportLogsToCsvAsync(
        [FromQuery] DateRangeRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var filePath = await httpRequestLogAppService.ExportLogsToCsvAsync(request, cancellationToken);
        if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
        {
            return NotFound("CSV file could not be generated or was not found");
        }

        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath, cancellationToken);
        var fileName = $"HttpRequestLogs_{request.StartDate:yyyyMMdd}-{request.EndDate:yyyyMMdd}.csv";

        return File(fileBytes, "text/csv", fileName);
    }
}