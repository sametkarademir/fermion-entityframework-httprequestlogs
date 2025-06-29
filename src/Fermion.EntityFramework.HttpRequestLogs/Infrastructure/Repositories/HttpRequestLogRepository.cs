using Fermion.EntityFramework.HttpRequestLogs.Core.Entities;
using Fermion.EntityFramework.HttpRequestLogs.Core.Interfaces.Repositories;
using Fermion.EntityFramework.Shared.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fermion.EntityFramework.HttpRequestLogs.Infrastructure.Repositories;

public class HttpRequestLogRepository<TContext> : EfRepositoryBase<HttpRequestLog, Guid, TContext>, IHttpRequestLogRepository where TContext : DbContext
{
    public HttpRequestLogRepository(TContext dbContext) : base(dbContext)
    {
    }
}