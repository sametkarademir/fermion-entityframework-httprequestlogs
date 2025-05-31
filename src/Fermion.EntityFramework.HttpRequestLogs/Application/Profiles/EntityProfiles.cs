using AutoMapper;
using Fermion.EntityFramework.HttpRequestLogs.Application.DTOs.HttpRequestLogs;
using Fermion.EntityFramework.HttpRequestLogs.Core.Entities;

namespace Fermion.EntityFramework.HttpRequestLogs.Application.Profiles;

public class EntityProfiles : Profile
{
    public EntityProfiles()
    {
        CreateMap<HttpRequestLog, HttpRequestLogResponseDto>();
    }
}