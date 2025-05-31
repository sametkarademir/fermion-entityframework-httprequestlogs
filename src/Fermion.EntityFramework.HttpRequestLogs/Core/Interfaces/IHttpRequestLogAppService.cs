using Fermion.EntityFramework.HttpRequestLogs.Application.DTOs.HttpRequestLogs;
using Fermion.EntityFramework.Shared.DTOs.Pagination;

namespace Fermion.EntityFramework.HttpRequestLogs.Core.Interfaces;

public interface IHttpRequestLogAppService
{
    Task<HttpRequestLogResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PageableResponseDto<HttpRequestLogResponseDto>> GetPageableAndFilterAsync(GetListHttpRequestLogRequestDto request, CancellationToken cancellationToken = default);
    Task<int> CleanupOldHttpRequestLogsAsync(DateTime olderThan, CancellationToken cancellationToken = default);

    Task<ResponseTimeStatsResponseDto> GetResponseTimeStatsAsync(ResponseTimeStatsRequestDto request, CancellationToken cancellationToken = default);
    Task<List<EndpointPerformanceResponseDto>> GetSlowestEndpointsAsync(EndpointPerformanceRequestDto request, CancellationToken cancellationToken = default);
    Task<List<EndpointUsageResponseDto>> GetMostFrequentEndpointsAsync(EndpointUsageRequestDto request, CancellationToken cancellationToken = default);

    Task<List<StatusCodeDistributionResponseDto>> GetStatusCodeDistributionAsync(StatusCodeDistributionRequestDto request, CancellationToken cancellationToken = default);
    Task<List<DefaultRateOverTimeResponseDto>> GetRequestRateOverTimeAsync(RateOverTimeRequestDto request, CancellationToken cancellationToken = default);
    Task<List<ErrorRateOverTimeResponseDto>> GetErrorRateOverTimeAsync(RateOverTimeRequestDto request, CancellationToken cancellationToken = default);

    Task<ClientUsageStatsResponseDto> GetClientUsageStatsAsync(DateRangeRequestDto request, CancellationToken cancellationToken = default);

    Task<int> GetTotalLogCountAsync(DateRangeRequestDto request, CancellationToken cancellationToken = default);
    Task<string> ExportLogsToCsvAsync(DateRangeRequestDto request, CancellationToken cancellationToken = default);
}