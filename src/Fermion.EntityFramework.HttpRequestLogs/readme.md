# Fermion.EntityFramework.HttpRequestLogs

Fermion.EntityFramework.HttpRequestLogs is a comprehensive HTTP request logging library for .NET applications that provides detailed tracking of HTTP requests, performance metrics, and request analysis. It's built on top of Entity Framework Core and follows clean architecture principles.

## Features

- Detailed HTTP request and response logging
- Performance metrics tracking
- Client device and browser information
- Request rate analysis
- Response time statistics
- Error rate monitoring
- Endpoint performance analysis
- Client usage statistics
- Configurable sensitive data masking
- Flexible request filtering
- CSV export functionality
- Automatic cleanup of old logs

## Installation

```bash
  dotnet add package Fermion.EntityFramework.HttpRequestLogs
```

## Project Structure

The library follows Clean Architecture principles with the following layers:

### Core
- Base entities and interfaces
- Domain models
- Enums and constants
- Configuration options

### Infrastructure
- Entity Framework Core configurations
- Database context implementations
- Repository implementations

### Application
- DTOs
- Interfaces
- Services
- Mappings

### Presentation
- Controllers
- API endpoints
- Request/Response models

### DependencyInjection
- Service registration extensions
- Configuration options
- Middleware implementation

## Configuration

```csharp
// Configure HTTP request logging in Startup.cs
builder.Services.AddFermionHttpRequestLogServices<ApplicationDbContext>(opt =>
{
    // Enable/Disable request logging
    opt.Enabled = true;

    // Configure path exclusions
    opt.ExcludedPaths = ["/health", "/metrics", "/favicon.ico"];
    opt.ExcludedHttpMethods = ["OPTIONS"];
    opt.ExcludedContentTypes = ["application/octet-stream", "application/pdf", "image/", "video/", "audio/"];

    // Configure request body logging
    opt.LogRequestBody = true;
    opt.MaxRequestBodyLength = 5000;
    opt.LogOnlySlowRequests = true;
    opt.SlowRequestThresholdMs = 10;

    // Configure sensitive data masking
    opt.MaskPattern = "***MASKED***";
    opt.RequestBodySensitiveProperties = [
        "Password", "Token", "Secret", "Key", "Credential", 
        "Ssn", "Credit", "Card", "Description"
    ];
    opt.QueryStringSensitiveProperties = [
        "Password", "Token", "Secret", "ApiKey", "Key"
    ];
    opt.HeaderSensitiveProperties = [
        "Authorization", "Cookie", "X-Api-Key"
    ];

    // Configure API endpoints
    opt.EnableApiEndpoints = true;
    opt.ApiRoute = "api/http-request-logs";
    
    // Configure authorization
    opt.Authorization.RequireAuthentication = true;
    opt.Authorization.GlobalPolicy = "HttpRequestLogPolicy";
    opt.Authorization.EndpointPolicies = ["Admin", "HttpRequestLog"];
});

// Configure DbContext
public class YourDbContext : DbContext 
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply HTTP request log configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HttpRequestLogConfiguration).Assembly);
    }
}

// Configure middleware in Program.cs
var app = builder.Build();

// Add HTTP request logging middleware (should be one of the first middleware)
app.FermionHttpRequestLogMiddleware();

// Add other middleware
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
```

## API Endpoints

The library provides the following RESTful API endpoints when `EnableApiEndpoints` is set to true:

- `GET /api/http-request-logs/{id}` - Get specific request log details
- `GET /api/http-request-logs/pageable` - Get request logs with filtering and pagination
- `DELETE /api/http-request-logs/cleanup` - Clean up old request logs
- `GET /api/http-request-logs/response-time-stats` - Get response time statistics
- `GET /api/http-request-logs/slowest-endpoints` - Get the slowest endpoints analysis
- `GET /api/http-request-logs/most-frequent-endpoints` - Get most frequent endpoints
- `GET /api/http-request-logs/status-code-distribution` - Get status code distribution
- `GET /api/http-request-logs/request-rate` - Get request rate over time
- `GET /api/http-request-logs/error-rate` - Get error rate over time
- `GET /api/http-request-logs/client-usage` - Get client usage statistics
- `GET /api/http-request-logs/total-count` - Get total log count
- `GET /api/http-request-logs/export` - Export logs to CSV

## DTOs

### HTTP Request Logs
- `HttpRequestLogResponseDto`: Detailed request log information
- `GetListHttpRequestLogRequestDto`: Filtering and pagination options

### Performance Analysis
- `ResponseTimeStatsRequestDto`: Response time statistics parameters
- `ResponseTimeStatsResponseDto`: Response time statistics results
- `EndpointPerformanceRequestDto`: Endpoint performance parameters
- `EndpointPerformanceResponseDto`: Endpoint performance results
- `EndpointUsageRequestDto`: Endpoint usage parameters
- `EndpointUsageResponseDto`: Endpoint usage results

### Rate Analysis
- `RateOverTimeRequestDto`: Rate analysis parameters
- `DefaultRateOverTimeResponseDto`: Request rate results
- `ErrorRateOverTimeResponseDto`: Error rate results

### Client Analysis
- `ClientUsageStatsResponseDto`: Client usage statistics
- `DateRangeRequestDto`: Date range parameters

## Enums

- `TimeInterval`: Time interval options (Minute, Hour, Day, Week, Month)
- `SortOrderTypes`: Sorting order options (Asc, Desc)