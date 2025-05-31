using System.Text.Json.Serialization;
using Fermion.EntityFramework.HttpRequestLogs.Core.Enums;
using FluentValidation;

namespace Fermion.EntityFramework.HttpRequestLogs.Application.DTOs.HttpRequestLogs;

public class RateOverTimeRequestDto : DateRangeRequestDto
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TimeInterval TimeInterval { get; set; } = TimeInterval.Day;
}

public class RateOverTimeRequestValidator : AbstractValidator<RateOverTimeRequestDto>
{
    public RateOverTimeRequestValidator()
    {
        RuleFor(x => x.TimeInterval)
            .IsInEnum().WithMessage("Invalid time interval value.")
            .Must(x => Enum.IsDefined(typeof(TimeInterval), x)).WithMessage("Invalid time interval value.");

        RuleFor(x => x)
            .Must(x => !(x.TimeInterval == TimeInterval.Minute || x.TimeInterval == TimeInterval.Hour) || x.StartDate.Date == x.EndDate.Date)
            .WithMessage("Start date and end date must be on the same day when minute or hour interval is selected.");
    }
}