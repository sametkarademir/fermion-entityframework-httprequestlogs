using System.Diagnostics;
using Fermion.EntityFramework.HttpRequestLogs.Core.Entities;
using Fermion.EntityFramework.HttpRequestLogs.Core.Interfaces.Repositories;
using Fermion.EntityFramework.HttpRequestLogs.Core.Options;
using Fermion.Extensions.Claims;
using Fermion.Extensions.HttpContexts;
using Fermion.Extensions.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fermion.EntityFramework.HttpRequestLogs.DependencyInjection;

public class HttpRequestMiddleware
{
    private readonly RequestDelegate _next;
    private readonly HttpRequestLogOptions _options;
    private readonly ILogger<HttpRequestMiddleware> _logger;

    public HttpRequestMiddleware(RequestDelegate next, IOptions<HttpRequestLogOptions> options, ILogger<HttpRequestMiddleware> logger)
    {
        _next = next;
        _options = options.Value;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context, IHttpRequestLogRepository httpRequestLogRepository)
    {
        if (!_options.Enabled)
        {
            await _next(context);
            return;
        }

        var path = context.GetPath().ToLowerInvariant();
        if (_options.ExcludedPaths.Any(item => path.StartsWith(item, StringComparison.OrdinalIgnoreCase)))
        {
            await _next(context);
            return;
        }

        if (_options.ExcludedHttpMethods.Contains(context.GetRequestMethod(), StringComparer.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var contentType = context.Request.ContentType?.ToLowerInvariant() ?? string.Empty;
        if (_options.ExcludedContentTypes.Any(ct => contentType.StartsWith(ct, StringComparison.OrdinalIgnoreCase)))
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var executionTime = DateTime.UtcNow;
        string? originalContent = null;
        if (_options.LogRequestBody)
        {
            originalContent = context.GetRequestBody(_options.MaxRequestBodyLength);
        }

        await _next(context);
        stopwatch.Stop();

        if (_options.LogOnlySlowRequests && stopwatch.ElapsedMilliseconds < _options.SlowRequestThresholdMs)
        {
            return;
        }

        var deviceInfo = context.GetDeviceInfo();

        var httpRequestLog = new HttpRequestLog
        {
            CreationTime = DateTime.UtcNow,
            CreatorId = context.User.GetUserIdToGuid(),
            HttpMethod = context.GetRequestMethod(),
            RequestPath = context.GetPath(),
            QueryString = JsonMaskExtensions.MaskSensitiveData(context.GetQueryStringToJson(),
                _options.MaskPattern,
                _options.QueryStringSensitiveProperties.ToArray()),
            RequestBody = JsonMaskExtensions.MaskSensitiveData(originalContent,
                _options.MaskPattern,
                _options.RequestBodySensitiveProperties.ToArray()),
            RequestHeaders = JsonMaskExtensions.MaskSensitiveData(context.GetRequestHeadersToJson(),
                _options.MaskPattern,
                _options.HeaderSensitiveProperties.ToArray()),
            StatusCode = context.Response.StatusCode,
            RequestTime = executionTime,
            ResponseTime = DateTime.UtcNow,
            DurationMs = stopwatch.ElapsedMilliseconds,
            ClientIp = context.GetClientIpAddress(),
            UserAgent = context.GetUserAgent(),
            DeviceFamily = deviceInfo.DeviceFamily,
            DeviceModel = deviceInfo.DeviceModel,
            OsFamily = deviceInfo.OsFamily,
            OsVersion = deviceInfo.OsVersion,
            BrowserFamily = deviceInfo.BrowserFamily,
            BrowserVersion = deviceInfo.BrowserVersion,
            ControllerName = context.GetControllerName(),
            ActionName = context.GetActionName(),
            SnapshotId = context.GetSnapshotId(),
            SessionId = context.GetSessionId(),
            CorrelationId = context.GetCorrelationId()
        };

        await httpRequestLogRepository.AddAsync(httpRequestLog);
        await httpRequestLogRepository.SaveChangesAsync();
    }
}