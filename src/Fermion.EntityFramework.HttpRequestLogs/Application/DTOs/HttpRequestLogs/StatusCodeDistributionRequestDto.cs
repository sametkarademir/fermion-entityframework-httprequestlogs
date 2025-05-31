using FluentValidation;

namespace Fermion.EntityFramework.HttpRequestLogs.Application.DTOs.HttpRequestLogs;

public class StatusCodeDistributionRequestDto : DateRangeRequestDto
{
}

public class StatusCodeDistributionRequestValidator : AbstractValidator<StatusCodeDistributionRequestDto>
{
    public StatusCodeDistributionRequestValidator()
    {
    }
}