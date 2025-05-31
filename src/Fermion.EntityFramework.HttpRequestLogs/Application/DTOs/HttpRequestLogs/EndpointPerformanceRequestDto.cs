using FluentValidation;

namespace Fermion.EntityFramework.HttpRequestLogs.Application.DTOs.HttpRequestLogs;

public class EndpointPerformanceRequestDto : DateRangeRequestDto
{
}

public class EndpointPerformanceRequestValidator : AbstractValidator<EndpointPerformanceRequestDto>
{
    public EndpointPerformanceRequestValidator()
    {
    }
}