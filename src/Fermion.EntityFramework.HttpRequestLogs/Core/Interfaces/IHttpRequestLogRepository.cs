using Fermion.EntityFramework.HttpRequestLogs.Core.Entities;
using Fermion.EntityFramework.Shared.Repositories.Abstractions;

namespace Fermion.EntityFramework.HttpRequestLogs.Core.Interfaces;

public interface IHttpRequestLogRepository : IRepository<HttpRequestLog, Guid>
{
}