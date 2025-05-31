using Microsoft.AspNetCore.Builder;

namespace Fermion.EntityFramework.HttpRequestLogs.DependencyInjection;

public static class ApplicationBuilderExceptionMiddlewareExtensions
{
    public static void FermionHttpRequestLogMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ProgressingStartedMiddleware>();
        app.UseMiddleware<HttpRequestMiddleware>();
    }
}