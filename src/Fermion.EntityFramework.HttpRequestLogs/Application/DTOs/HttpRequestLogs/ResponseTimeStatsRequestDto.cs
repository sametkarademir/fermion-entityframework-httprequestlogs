using FluentValidation;

namespace Fermion.EntityFramework.HttpRequestLogs.Application.DTOs.HttpRequestLogs;

public class ResponseTimeStatsRequestDto : DateRangeRequestDto
{
    public string Path { get; set; } = null!;
}

public class ResponseTimeStatsRequestValidator : AbstractValidator<ResponseTimeStatsRequestDto>
{
    public ResponseTimeStatsRequestValidator()
    {
        RuleFor(x => x.Path)
            .NotEmpty().WithMessage("Path is required.")
            .MaximumLength(1000).WithMessage("Path must not exceed 1000 characters.");
    }
}