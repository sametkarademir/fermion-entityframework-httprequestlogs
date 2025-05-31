using Fermion.Extensions.HttpContexts;
using Microsoft.AspNetCore.Http;

namespace Fermion.EntityFramework.HttpRequestLogs.DependencyInjection;

public class ProgressingStartedMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        context.SetCorrelationId(Guid.NewGuid());
        await next(context);
    }
}