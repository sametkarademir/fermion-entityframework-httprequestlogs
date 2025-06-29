using Fermion.EntityFramework.HttpRequestLogs.Application.DTOs.HttpRequestLogs;
using Fermion.EntityFramework.HttpRequestLogs.Core.Interfaces;
using Fermion.EntityFramework.HttpRequestLogs.Core.Interfaces.Services;
using Fermion.EntityFramework.Shared.DTOs.Pagination;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fermion.EntityFramework.HttpRequestLogs.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HttpRequestLogController(
    IHttpRequestLogAppService httpRequestLogAppService)
    : ControllerBase
{
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(HttpRequestLogResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await httpRequestLogAppService.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpGet("pageable")]
    [ProducesResponseType(typeof(PageableResponseDto<HttpRequestLogResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPageableAndFilterAsync(
        [FromQuery] GetListHttpRequestLogRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var result = await httpRequestLogAppService.GetPageableAndFilterAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("cleanup")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<ActionResult> CleanupOldHttpRequestLogsAsync(
        [FromQuery] DateTime olderThan,
        CancellationToken cancellationToken = default)
    {
        var result = await httpRequestLogAppService.CleanupOldHttpRequestLogsAsync(olderThan, cancellationToken);
        return Ok(result);
    }

    [HttpGet("response-time-stats")]
    [ProducesResponseType(typeof(ResponseTimeStatsResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetResponseTimeStatsAsync(
        [FromQuery] ResponseTimeStatsRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var result = await httpRequestLogAppService.GetResponseTimeStatsAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("slowest-endpoints")]
    [ProducesResponseType(typeof(List<EndpointPerformanceResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetSlowestEndpointsAsync(
        [FromQuery] EndpointPerformanceRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var result = await httpRequestLogAppService.GetSlowestEndpointsAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("most-frequent-endpoints")]
    [ProducesResponseType(typeof(List<EndpointUsageResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetMostFrequentEndpointsAsync(
        [FromQuery] EndpointUsageRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var result = await httpRequestLogAppService.GetMostFrequentEndpointsAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("status-code-distribution")]
    [ProducesResponseType(typeof(List<StatusCodeDistributionResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetStatusCodeDistributionAsync(
        [FromQuery] StatusCodeDistributionRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var result = await httpRequestLogAppService.GetStatusCodeDistributionAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("request-rate")]
    [ProducesResponseType(typeof(List<DefaultRateOverTimeResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetRequestRateOverTimeAsync(
        [FromQuery] RateOverTimeRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var result = await httpRequestLogAppService.GetRequestRateOverTimeAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("error-rate")]
    [ProducesResponseType(typeof(List<ErrorRateOverTimeResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetErrorRateOverTimeAsync(
        [FromQuery] RateOverTimeRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var result = await httpRequestLogAppService.GetErrorRateOverTimeAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("client-usage-stats")]
    [ProducesResponseType(typeof(ClientUsageStatsResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetClientUsageStatsAsync(
        [FromQuery] DateRangeRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var result = await httpRequestLogAppService.GetClientUsageStatsAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("count")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
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