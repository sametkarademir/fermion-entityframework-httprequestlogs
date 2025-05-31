namespace Fermion.EntityFramework.HttpRequestLogs.Core.Options;

public class HttpRequestLogOptions
{
    public bool Enabled { get; set; } = true;
    public List<string> ExcludedPaths { get; set; } = ["/health", "/metrics", "/favicon.ico"];
    public List<string> ExcludedHttpMethods { get; set; } = [];
    public List<string> ExcludedContentTypes { get; set; } = ["application/octet-stream", "application/pdf", "image/", "video/", "audio/"];

    public bool LogRequestBody { get; set; } = true;
    public int MaxRequestBodyLength { get; set; } = 5000;
    public bool LogOnlySlowRequests { get; set; } = true;
    public long SlowRequestThresholdMs { get; set; } = 10;

    public string MaskPattern { get; set; } = "***MASKED***";
    public List<string> RequestBodySensitiveProperties { get; set; } = ["Password", "Token", "Secret", "Key", "Credential", "Ssn", "Credit", "Card"];
    public List<string> QueryStringSensitiveProperties { get; set; } = ["Password", "Token", "Secret", "ApiKey", "Key"];
    public List<string> HeaderSensitiveProperties { get; set; } = ["Authorization", "Cookie", "X-Api-Key"];

    public bool EnableApiEndpoints { get; set; } = true;
    public string ApiRoute { get; set; } = "api/http-request-logs";
    public AuthorizationOptions Authorization { get; set; } = new AuthorizationOptions();
}

public class AuthorizationOptions
{
    /// <summary>
    /// Global authorization policy to apply to all endpoints
    /// </summary>
    public string? GlobalPolicy { get; set; }

    /// <summary>
    /// If true, all endpoints require authentication even if no policy is specified
    /// </summary>
    public bool RequireAuthentication { get; set; } = true;

    /// <summary>
    /// Endpoint-specific authorization policies (overrides global policy)
    /// Key: Endpoint method name, Value: Policy name (or null to use global policy)
    /// </summary>
    public List<string>? EndpointPolicies { get; set; }
}