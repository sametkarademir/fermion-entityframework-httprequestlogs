using System.Text;
using AutoMapper;
using Fermion.EntityFramework.HttpRequestLogs.Application.DTOs.HttpRequestLogs;
using Fermion.EntityFramework.HttpRequestLogs.Core.Enums;
using Fermion.EntityFramework.HttpRequestLogs.Core.Interfaces;
using Fermion.EntityFramework.Shared.DTOs.Pagination;
using Fermion.EntityFramework.Shared.Extensions;
using Fermion.Extensions.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fermion.EntityFramework.HttpRequestLogs.Application.Services;

public class HttpRequestLogAppService(
    IHttpRequestLogRepository httpRequestLogRepository,
    IMapper mapper,
    ILogger<HttpRequestLogAppService> logger)
    : IHttpRequestLogAppService
{
    public async Task<HttpRequestLogResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var matchedHttpRequestLog = await httpRequestLogRepository.GetAsync(
            id: id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );

        return mapper.Map<HttpRequestLogResponseDto>(matchedHttpRequestLog);
    }

    public async Task<PageableResponseDto<HttpRequestLogResponseDto>> GetPageableAndFilterAsync(GetListHttpRequestLogRequestDto request, CancellationToken cancellationToken = default)
    {
        var queryable = httpRequestLogRepository.Query();
        queryable = queryable.WhereIf(!string.IsNullOrWhiteSpace(request.ClientIp), item => item.ClientIp == request.ClientIp);
        queryable = queryable.WhereIf(!string.IsNullOrWhiteSpace(request.ControllerName), item => item.ControllerName == request.ControllerName);
        queryable = queryable.WhereIf(!string.IsNullOrWhiteSpace(request.ActionName), item => item.ActionName == request.ActionName);
        queryable = queryable.WhereIf(request.StatusCode.HasValue, p => p.StatusCode == request.StatusCode);
        queryable = queryable.WhereIf(!string.IsNullOrWhiteSpace(request.HttpMethod), item => item.HttpMethod == request.HttpMethod);
        queryable = queryable.WhereIf(!string.IsNullOrWhiteSpace(request.DeviceFamily), item => item.DeviceFamily == request.DeviceFamily);
        queryable = queryable.WhereIf(!string.IsNullOrWhiteSpace(request.DeviceModel), item => item.DeviceModel == request.DeviceModel);
        queryable = queryable.WhereIf(!string.IsNullOrWhiteSpace(request.OsFamily), item => item.OsFamily == request.OsFamily);
        queryable = queryable.WhereIf(!string.IsNullOrWhiteSpace(request.OsVersion), item => item.OsVersion == request.OsVersion);
        queryable = queryable.WhereIf(!string.IsNullOrWhiteSpace(request.BrowserFamily), item => item.BrowserFamily == request.BrowserFamily);
        queryable = queryable.WhereIf(!string.IsNullOrWhiteSpace(request.BrowserVersion), item => item.BrowserVersion == request.BrowserVersion);
        queryable = queryable.WhereIf(request.StartDate.HasValue, item => item.CreationTime >= request.StartDate);
        queryable = queryable.WhereIf(request.EndDate.HasValue, item => item.CreationTime >= request.EndDate);
        queryable = queryable.WhereIf(request.GreaterThanDurationMs.HasValue, item => item.DurationMs > request.GreaterThanDurationMs);
        queryable = queryable.WhereIf(request.LessThanDurationMs.HasValue, item => item.DurationMs < request.LessThanDurationMs);
        queryable = queryable.WhereIf(request.SnapshotId.HasValue, item => item.SnapshotId == request.SnapshotId);
        queryable = queryable.WhereIf(request.SessionId.HasValue, item => item.SessionId == request.SessionId);
        queryable = queryable.WhereIf(request.CorrelationId.HasValue, item => item.CorrelationId == request.CorrelationId);
        queryable = queryable.ApplySort(request.Field, request.Order, cancellationToken);

        queryable = queryable.AsNoTracking();
        var result = await queryable.ToPageableAsync(request.Page, request.PerPage, cancellationToken: cancellationToken);
        var mappedHttpRequestLogs = mapper.Map<List<HttpRequestLogResponseDto>>(result.Data);

        return new PageableResponseDto<HttpRequestLogResponseDto>(mappedHttpRequestLogs, result.Meta);
    }

    public async Task<int> CleanupOldHttpRequestLogsAsync(DateTime olderThan, CancellationToken cancellationToken = default)
    {
        var queryable = httpRequestLogRepository.Query();
        queryable = queryable.Where(x => x.CreationTime < olderThan);
        var countToDelete = await queryable.CountAsync(cancellationToken: cancellationToken);
        if (countToDelete == 0)
        {
            return 0;
        }

        const int batchSize = 200;
        var totalDeleted = 0;
        while (countToDelete > totalDeleted)
        {
            try
            {
                logger.LogInformation(
                    "[CleanupOldHttpRequestLogsAsync] [Action=DeleteRangeAsync()] [Count={Count}] [Start]",
                    countToDelete - totalDeleted
                );

                var httpRequestLogsToDelete = await queryable
                    .OrderBy(x => x.CreationTime)
                    .Take(batchSize)
                    .ToListAsync(cancellationToken: cancellationToken);

                if (httpRequestLogsToDelete.Count == 0)
                {
                    logger.LogInformation(
                        "[CleanupOldHttpRequestLogsAsync] [Action=DeleteRangeAsync()] [Count={Count}] [NoMoreLogsToDelete]",
                        totalDeleted
                    );

                    break;
                }

                await httpRequestLogRepository.DeleteRangeAsync(httpRequestLogsToDelete);
                await httpRequestLogRepository.SaveChangesAsync(cancellationToken);

                logger.LogInformation(
                    "[CleanupOldHttpRequestLogsAsync] [Action=DeleteRangeAsync()] [Count={Count}] [End]",
                    httpRequestLogsToDelete.Count
                );

                totalDeleted += httpRequestLogsToDelete.Count;

                if (cancellationToken.IsCancellationRequested)
                {
                    logger.LogInformation(
                        "[CleanupOldAuditLogsAsync] [Action=DeleteRangeAsync()] [Cancelled] [TotalDeleted={TotalDeleted}]",
                        totalDeleted
                    );

                    break;
                }

                if (totalDeleted > 0 && totalDeleted % (batchSize * 5) == 0)
                {
                    await Task.Delay(500, cancellationToken);
                }
            }
            catch (Exception e)
            {
                logger.LogError(
                    e,
                    "[CleanupOldHttpRequestLogsAsync] [Action=DeleteRangeAsync()] [Error] [Exception={Exception}]",
                    e.Message
                );

                break;
            }
        }

        return totalDeleted;
    }

    public async Task<ResponseTimeStatsResponseDto> GetResponseTimeStatsAsync(ResponseTimeStatsRequestDto request, CancellationToken cancellationToken = default)
    {
        var queryable = httpRequestLogRepository.Query();
        queryable = queryable.Where(item => item.RequestPath == request.Path);
        queryable = queryable.Where(item => item.RequestTime.Date >= request.StartDate.Date);
        queryable = queryable.Where(item => item.RequestTime.Date <= request.EndDate.Date);
        queryable = queryable.Where(item => item.DurationMs.HasValue);
        queryable = queryable.AsNoTracking();
        var httpRequestLogs = await queryable.ToListAsync(cancellationToken: cancellationToken);

        if (httpRequestLogs.Count == 0)
        {
            return new ResponseTimeStatsResponseDto
            {
                Path = request.Path,
                RequestCount = 0,
                MinResponseTime = 0,
                MaxResponseTime = 0,
                AverageResponseTime = 0,
                MedianResponseTime = 0,
                Percentile90 = 0,
                Percentile95 = 0,
                Percentile99 = 0
            };
        }

        var responseTimes = httpRequestLogs
            .Where(log => log.DurationMs.HasValue)
            .Select(log => log.DurationMs!.Value)
            .OrderBy(time => time)
            .ToList();

        var count = responseTimes.Count;

        var stats = new ResponseTimeStatsResponseDto
        {
            Path = request.Path,
            RequestCount = count,
            MinResponseTime = responseTimes.First(),
            MaxResponseTime = responseTimes.Last(),
            AverageResponseTime = responseTimes.Average(),

            MedianResponseTime = CalculatePercentile(responseTimes, 50),

            Percentile90 = CalculatePercentile(responseTimes, 90),
            Percentile95 = CalculatePercentile(responseTimes, 95),
            Percentile99 = CalculatePercentile(responseTimes, 99)
        };

        return stats;
    }

    public async Task<List<EndpointPerformanceResponseDto>> GetSlowestEndpointsAsync(EndpointPerformanceRequestDto request, CancellationToken cancellationToken = default)
    {
        var queryable = httpRequestLogRepository.Query();
        queryable = queryable.Where(item => item.RequestTime.Date >= request.StartDate.Date);
        queryable = queryable.Where(item => item.RequestTime.Date <= request.EndDate.Date);
        queryable = queryable.Where(item => item.DurationMs.HasValue);
        queryable = queryable.AsNoTracking();
        var httpRequestLogs = await queryable.ToListAsync(cancellationToken: cancellationToken);

        if (httpRequestLogs.Count == 0)
        {
            return [];
        }

        var endpointStats = httpRequestLogs
            .GroupBy(log => log.RequestPath)
            .Select(group =>
            {
                var errorCount = group.Count(log => log.StatusCode is >= 400);
                var totalRequests = group.Count();
                var avgResponseTime = group
                    .Where(log => log.DurationMs.HasValue)
                    .Average(log => log.DurationMs!.Value);

                var errorRate = totalRequests > 0
                    ? (double)errorCount / totalRequests * 100
                    : 0;

                return new EndpointPerformanceResponseDto
                {
                    Path = group.Key ?? "Unknown",
                    AverageResponseTime = avgResponseTime,
                    RequestCount = totalRequests,
                    ErrorRate = errorRate
                };
            })
            .OrderByDescending(stats => stats.AverageResponseTime)
            .Take(10)
            .ToList();

        return endpointStats;
    }

    public async Task<List<EndpointUsageResponseDto>> GetMostFrequentEndpointsAsync(EndpointUsageRequestDto request, CancellationToken cancellationToken = default)
    {
        var queryable = httpRequestLogRepository.Query();
        queryable = queryable.Where(item => item.RequestTime.Date >= request.StartDate.Date);
        queryable = queryable.Where(item => item.RequestTime.Date <= request.EndDate.Date);
        queryable = queryable.AsNoTracking();
        var httpRequestLogs = await queryable.ToListAsync(cancellationToken: cancellationToken);

        if (httpRequestLogs.Count == 0)
        {
            return [];
        }

        var endpointUsage = httpRequestLogs
            .GroupBy(log => log.RequestPath)
            .Select(group =>
            {
                var requestCount = group.Count();
                var avgResponseTime = group
                    .Where(log => log.DurationMs.HasValue)
                    .Select(log => log.DurationMs!.Value)
                    .DefaultIfEmpty(0)
                    .Average();

                return new EndpointUsageResponseDto
                {
                    Path = group.Key ?? "Unknown",
                    RequestCount = requestCount,
                    AverageResponseTime = avgResponseTime
                };
            })
            .OrderByDescending(usage => usage.RequestCount)
            .Take(10)
            .ToList();

        return endpointUsage;
    }

    public async Task<List<StatusCodeDistributionResponseDto>> GetStatusCodeDistributionAsync(StatusCodeDistributionRequestDto request, CancellationToken cancellationToken = default)
    {
        var queryable = httpRequestLogRepository.Query();
        queryable = queryable.Where(item => item.RequestTime.Date >= request.StartDate.Date);
        queryable = queryable.Where(item => item.RequestTime.Date <= request.EndDate.Date);
        queryable = queryable.Where(item => item.StatusCode.HasValue);
        queryable = queryable.AsNoTracking();
        var httpRequestLogs = await queryable.ToListAsync(cancellationToken: cancellationToken);

        if (httpRequestLogs.Count == 0)
        {
            return [];
        }

        var statusCodeDistribution = httpRequestLogs
            .Where(log => log.StatusCode.HasValue)
            .GroupBy(log => log.StatusCode!.Value)
            .ToDictionary(
                group => group.Key,
                group => group.Count()
            );

        return statusCodeDistribution
            .OrderBy(kvp => kvp.Key)
            .Select(kvp => new StatusCodeDistributionResponseDto
            {
                StatusCode = kvp.Key,
                Count = kvp.Value
            }).ToList();
    }

    public async Task<List<DefaultRateOverTimeResponseDto>> GetRequestRateOverTimeAsync(RateOverTimeRequestDto request, CancellationToken cancellationToken = default)
    {
        var queryable = httpRequestLogRepository.Query();
        queryable = queryable.Where(item => item.RequestTime.Date >= request.StartDate.Date);
        queryable = queryable.Where(item => item.RequestTime.Date <= request.EndDate.Date);
        queryable = queryable.AsNoTracking();
        var httpRequestLogs = await queryable.ToListAsync(cancellationToken: cancellationToken);

        if (httpRequestLogs.Count == 0)
        {
            return [];
        }

        Func<DateTime, DateTime> truncateDate = request.TimeInterval switch
        {
            TimeInterval.Minute => date => new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0),
            TimeInterval.Hour => date => new DateTime(date.Year, date.Month, date.Day, date.Hour, 0, 0),
            TimeInterval.Day => date => date.Date,
            TimeInterval.Week => date => date.Date.AddDays(-(int)date.DayOfWeek),
            TimeInterval.Month => date => new DateTime(date.Year, date.Month, 1),
            _ => date => date.Date
        };

        var requestRateOverTime = httpRequestLogs
            .GroupBy(log => truncateDate(log.RequestTime))
            .OrderBy(group => group.Key)
            .ToDictionary(
                group => group.Key,
                group => group.Count()
            );

        var result = new List<DefaultRateOverTimeResponseDto>();

        var currentDate = truncateDate(request.StartDate);
        var alignedEndDate = truncateDate(request.EndDate);
        while (currentDate <= alignedEndDate)
        {
            if (requestRateOverTime.TryGetValue(currentDate, out var count))
            {
                result.Add(new DefaultRateOverTimeResponseDto
                {
                    DateTime = currentDate,
                    Count = count
                });
            }
            else
            {
                result.Add(new DefaultRateOverTimeResponseDto
                {
                    DateTime = currentDate,
                    Count = 0
                });
            }

            currentDate = request.TimeInterval switch
            {
                TimeInterval.Minute => currentDate.AddMinutes(1),
                TimeInterval.Hour => currentDate.AddHours(1),
                TimeInterval.Day => currentDate.AddDays(1),
                TimeInterval.Week => currentDate.AddDays(7),
                TimeInterval.Month => currentDate.AddMonths(1),
                _ => currentDate.AddDays(1)
            };
        }

        return result;
    }

    public async Task<List<ErrorRateOverTimeResponseDto>> GetErrorRateOverTimeAsync(RateOverTimeRequestDto request, CancellationToken cancellationToken = default)
    {
        var queryable = httpRequestLogRepository.Query();
        queryable = queryable.Where(item => item.RequestTime.Date >= request.StartDate.Date);
        queryable = queryable.Where(item => item.RequestTime.Date <= request.EndDate.Date);
        queryable = queryable.AsNoTracking();
        var httpRequestLogs = await queryable.ToListAsync(cancellationToken: cancellationToken);

        if (httpRequestLogs.Count == 0)
        {
            return [];
        }

        Func<DateTime, DateTime> truncateDate = request.TimeInterval switch
        {
            TimeInterval.Minute => date => new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0),
            TimeInterval.Hour => date => new DateTime(date.Year, date.Month, date.Day, date.Hour, 0, 0),
            TimeInterval.Day => date => date.Date,
            TimeInterval.Week => date => date.Date.AddDays(-(int)date.DayOfWeek),
            TimeInterval.Month => date => new DateTime(date.Year, date.Month, 1),
            _ => date => date.Date
        };

        var errorRateOverTime = httpRequestLogs
            .GroupBy(log => truncateDate(log.RequestTime))
            .OrderBy(group => group.Key)
            .ToDictionary(
                group => group.Key,
                group =>
                {
                    var totalCount = group.Count();
                    var errorCount = group.Count(log => log.StatusCode is >= 400);

                    return (ErrorCount: errorCount, TotalCount: totalCount);
                }
            );

        var result = new List<ErrorRateOverTimeResponseDto>();
        var currentDate = truncateDate(request.StartDate);
        var alignedEndDate = truncateDate(request.EndDate);

        while (currentDate <= alignedEndDate)
        {
            var errorRate = 0.0;
            var totalCount = 0;
            if (errorRateOverTime.TryGetValue(currentDate, out var counts))
            {
                errorRate = counts.ErrorCount / (double)counts.TotalCount * 100;
                totalCount = counts.TotalCount;
            }
            result.Add(new ErrorRateOverTimeResponseDto
            {
                DateTime = currentDate,
                ErrorRate = (int)errorRate,
                TotalCount = totalCount
            });

            currentDate = request.TimeInterval switch
            {
                TimeInterval.Minute => currentDate.AddMinutes(1),
                TimeInterval.Hour => currentDate.AddHours(1),
                TimeInterval.Day => currentDate.AddDays(1),
                TimeInterval.Week => currentDate.AddDays(7),
                TimeInterval.Month => currentDate.AddMonths(1),
                _ => currentDate.AddDays(1)
            };
        }

        return result;
    }

    public async Task<ClientUsageStatsResponseDto> GetClientUsageStatsAsync(DateRangeRequestDto request, CancellationToken cancellationToken = default)
    {
        var queryable = httpRequestLogRepository.Query();
        queryable = queryable.Where(item => item.RequestTime.Date >= request.StartDate.Date);
        queryable = queryable.Where(item => item.RequestTime.Date <= request.EndDate.Date);
        queryable = queryable.AsNoTracking();
        var httpRequestLogs = await queryable.ToListAsync(cancellationToken: cancellationToken);

        if (httpRequestLogs.Count == 0)
        {
            return new ClientUsageStatsResponseDto
            {
                BrowserFamilies = new Dictionary<string, int>(),
                OsFamilies = new Dictionary<string, int>(),
                DeviceFamilies = new Dictionary<string, int>()
            };
        }

        var browserFamilies = httpRequestLogs
            .Where(log => !string.IsNullOrEmpty(log.BrowserFamily))
            .GroupBy(log => log.BrowserFamily!)
            .OrderByDescending(group => group.Count())
            .ToDictionary(
                group => group.Key,
                group => group.Count()
            );

        var osFamilies = httpRequestLogs
            .Where(log => !string.IsNullOrEmpty(log.OsFamily))
            .GroupBy(log => log.OsFamily!)
            .OrderByDescending(group => group.Count())
            .ToDictionary(
                group => group.Key,
                group => group.Count()
            );

        var deviceFamilies = httpRequestLogs
            .Where(log => !string.IsNullOrEmpty(log.DeviceFamily))
            .GroupBy(log => log.DeviceFamily!)
            .OrderByDescending(group => group.Count())
            .ToDictionary(
                group => group.Key,
                group => group.Count()
            );

        var browserVersions = httpRequestLogs
            .Where(log => !string.IsNullOrEmpty(log.BrowserFamily) && !string.IsNullOrEmpty(log.BrowserVersion))
            .GroupBy(log => $"{log.BrowserFamily} {log.BrowserVersion}")
            .OrderByDescending(group => group.Count())
            .Take(10)
            .ToDictionary(
                group => group.Key,
                group => group.Count()
            );

        var osVersions = httpRequestLogs
            .Where(log => !string.IsNullOrEmpty(log.OsFamily) && !string.IsNullOrEmpty(log.OsVersion))
            .GroupBy(log => $"{log.OsFamily} {log.OsVersion}")
            .OrderByDescending(group => group.Count())
            .Take(10)
            .ToDictionary(
                group => group.Key,
                group => group.Count()
            );

        bool IsMobileDevice(string? deviceFamily)
        {
            if (string.IsNullOrEmpty(deviceFamily))
                return false;

            return deviceFamily.Contains("Mobile") ||
                   deviceFamily.Contains("iPhone") ||
                   deviceFamily.Contains("iPad") ||
                   deviceFamily.Contains("Android") ||
                   deviceFamily.Contains("Windows Phone");
        }

        var mobileCount = httpRequestLogs.Count(log => IsMobileDevice(log.DeviceFamily));
        var desktopCount = httpRequestLogs.Count() - mobileCount;

        var deviceTypeDistribution = new Dictionary<string, int>
        {
            { "Mobile", mobileCount },
            { "Desktop", desktopCount }
        };

        var clientUsageStats = new ClientUsageStatsResponseDto
        {
            BrowserFamilies = browserFamilies,
            OsFamilies = osFamilies,
            DeviceFamilies = deviceFamilies,
            BrowserVersions = browserVersions,
            OsVersions = osVersions,
            DeviceTypeDistribution = deviceTypeDistribution,
            TotalRequests = httpRequestLogs.Count()
        };

        return clientUsageStats;
    }

    public async Task<int> GetTotalLogCountAsync(DateRangeRequestDto request, CancellationToken cancellationToken = default)
    {
        var count = await httpRequestLogRepository.CountAsync(
            predicate: log =>
                log.RequestTime >= request.StartDate &&
                log.RequestTime <= request.EndDate,
            cancellationToken: cancellationToken
        );

        return count;
    }

    public async Task<string> ExportLogsToCsvAsync(DateRangeRequestDto request, CancellationToken cancellationToken = default)
    {
        var queryable = httpRequestLogRepository.Query();
        queryable = queryable.Where(item => item.RequestTime.Date >= request.StartDate.Date);
        queryable = queryable.Where(item => item.RequestTime.Date <= request.EndDate.Date);
        queryable = queryable.AsNoTracking();
        var httpRequestLogs = await queryable.ToListAsync(cancellationToken: cancellationToken);

        if (httpRequestLogs.Count == 0)
        {
            return string.Empty;
        }

        var fileName = $"HttpRequestLogs_{request.StartDate:yyyyMMdd}-{request.EndDate:yyyyMMdd}_{Guid.NewGuid()}.csv";
        var tempPath = Path.Combine(Path.GetTempPath(), fileName);

        var headers = new List<string>
        {
            "Id", "RequestTime", "ResponseTime", "DurationMs", "HttpMethod",
            "RequestPath", "StatusCode", "ClientIp", "UserAgent",
            "DeviceFamily", "OsFamily", "BrowserFamily", "ServiceName",
            "ControllerName", "ActionName", "CorrelationId", "SessionId", "SnapshotId"
        };

        await using var writer = new StreamWriter(tempPath, false, Encoding.UTF8);
        await writer.WriteLineAsync(string.Join(",", headers.Select(EscapeCsvField)));
        foreach (var log in httpRequestLogs)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            var values = new List<string>
            {
                EscapeCsvField(log.Id.ToString()),
                EscapeCsvField(log.RequestTime.ToString("yyyy-MM-dd HH:mm:ss")),
                EscapeCsvField(log.ResponseTime.ToString("yyyy-MM-dd HH:mm:ss")),
                EscapeCsvField(log.DurationMs?.ToString() ?? ""),
                EscapeCsvField(log.HttpMethod ?? ""),
                EscapeCsvField(log.RequestPath ?? ""),
                EscapeCsvField(log.StatusCode?.ToString() ?? ""),
                EscapeCsvField(log.ClientIp ?? ""),
                EscapeCsvField(log.UserAgent ?? ""),
                EscapeCsvField(log.DeviceFamily ?? ""),
                EscapeCsvField(log.OsFamily ?? ""),
                EscapeCsvField(log.BrowserFamily ?? ""),
                EscapeCsvField(log.ControllerName ?? ""),
                EscapeCsvField(log.ActionName ?? ""),
                EscapeCsvField(log.CorrelationId?.ToString() ?? ""),
                EscapeCsvField(log.SessionId?.ToString() ?? ""),
                EscapeCsvField(log.SnapshotId?.ToString() ?? "")
            };

            await writer.WriteLineAsync(string.Join(",", values));
        }

        if (cancellationToken.IsCancellationRequested && File.Exists(tempPath))
        {
            File.Delete(tempPath);
            throw new OperationCanceledException("Export operation was canceled.");
        }

        return tempPath;
    }

    private long CalculatePercentile(List<long> sortedValues, int percentile)
    {
        switch (sortedValues.Count)
        {
            case 0:
                return 0;
            case 1:
                return sortedValues[0];
        }

        double n = sortedValues.Count;
        double pos = (n * percentile) / 100.0;

        if (Math.Abs(pos - Math.Floor(pos)) < 0.001)
        {
            return sortedValues[(int)pos - 1];
        }
        else
        {
            int lower = (int)Math.Floor(pos) - 1;
            int upper = (int)Math.Ceiling(pos) - 1;

            lower = Math.Max(0, lower);
            upper = Math.Min(sortedValues.Count - 1, upper);

            double weight = pos - Math.Floor(pos);
            return (long)((1 - weight) * sortedValues[lower] + weight * sortedValues[upper]);
        }
    }
    private string EscapeCsvField(string field)
    {
        if (string.IsNullOrEmpty(field))
        {
            return string.Empty;
        }

        if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }

        return field;
    }
}