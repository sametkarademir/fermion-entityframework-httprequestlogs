using Fermion.EntityFramework.HttpRequestLogs.Core.Entities;
using Fermion.EntityFramework.Shared.Interfaces;

namespace Fermion.EntityFramework.HttpRequestLogs.Core.Interfaces.Repositories;

public interface IHttpRequestLogRepository : IRepository<HttpRequestLog, Guid>
{
}