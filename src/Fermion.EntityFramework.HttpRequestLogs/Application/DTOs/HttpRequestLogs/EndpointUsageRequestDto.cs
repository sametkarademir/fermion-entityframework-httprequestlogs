using FluentValidation;

namespace Fermion.EntityFramework.HttpRequestLogs.Application.DTOs.HttpRequestLogs;

public class EndpointUsageRequestDto : DateRangeRequestDto
{
}

public class EndpointUsageRequestValidator : AbstractValidator<EndpointUsageRequestDto>
{
    public EndpointUsageRequestValidator()
    {
    }
}