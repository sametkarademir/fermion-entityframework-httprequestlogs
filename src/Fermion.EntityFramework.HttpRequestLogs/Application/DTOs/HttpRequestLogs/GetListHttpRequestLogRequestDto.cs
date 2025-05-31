using System.Text.Json.Serialization;
using Fermion.EntityFramework.Shared.DTOs.Sorting;
using FluentValidation;

namespace Fermion.EntityFramework.HttpRequestLogs.Application.DTOs.HttpRequestLogs;

public class GetListHttpRequestLogRequestDto
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 25;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SortOrderTypes Order { get; set; } = SortOrderTypes.Desc;
    public string? Field { get; set; } = null;

    public string? ClientIp { get; set; }
    public string? ControllerName { get; set; }
    public string? ActionName { get; set; }
    public int? StatusCode { get; set; }
    public string? HttpMethod { get; set; }

    public string? DeviceFamily { get; set; }
    public string? DeviceModel { get; set; }
    public string? OsFamily { get; set; }
    public string? OsVersion { get; set; }
    public string? BrowserFamily { get; set; }
    public string? BrowserVersion { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public long? GreaterThanDurationMs { get; set; }
    public long? LessThanDurationMs { get; set; }

    public Guid? SnapshotId { get; set; }
    public Guid? SessionId { get; set; }
    public Guid? CorrelationId { get; set; }
}

public class GetListHttpRequestLogRequestValidator : AbstractValidator<GetListHttpRequestLogRequestDto>
{
    public GetListHttpRequestLogRequestValidator()
    {
        RuleFor(x => x.ClientIp)
            .MaximumLength(50).WithMessage("Client IP cannot exceed 50 characters.");

        RuleFor(x => x.ControllerName)
            .MaximumLength(500).WithMessage("Controller Name cannot exceed 500 characters.");

        RuleFor(x => x.ActionName)
            .MaximumLength(500).WithMessage("Action Name cannot exceed 500 characters.");

        RuleFor(x => x.HttpMethod)
            .MaximumLength(10).WithMessage("HTTP Method cannot exceed 10 characters.");

        RuleFor(x => x.StatusCode)
            .Must(x => x == null || (x >= 100 && x <= 599)).WithMessage("Status Code must be between 100 and 599.");

        RuleFor(x => x.StartDate)
            .LessThan(x => x.EndDate).WithMessage("Start date must be less than end date.")
            .LessThanOrEqualTo(DateTime.Today).WithMessage("Start date cannot be in the future.");

        RuleFor(x => x.EndDate)
            .LessThanOrEqualTo(DateTime.Today).WithMessage("End date cannot be in the future.")
            .GreaterThan(x => x.StartDate).WithMessage("End date must be greater than start date.");

        RuleFor(x => x.DeviceFamily)
            .MaximumLength(100).WithMessage("Device Family cannot exceed 100 characters.");

        RuleFor(x => x.DeviceModel)
            .MaximumLength(100).WithMessage("Device Model cannot exceed 100 characters.");

        RuleFor(x => x.OsFamily)
            .MaximumLength(100).WithMessage("OS Family cannot exceed 100 characters.");

        RuleFor(x => x.OsVersion)
            .MaximumLength(100).WithMessage("OS Version cannot exceed 100 characters.");

        RuleFor(x => x.BrowserFamily)
            .MaximumLength(100).WithMessage("Browser Family cannot exceed 100 characters.");

        RuleFor(x => x.BrowserVersion)
            .MaximumLength(100).WithMessage("Browser Version cannot exceed 100 characters.");

        RuleFor(x => x.GreaterThanDurationMs)
            .Must(x => x == null || x > 0).WithMessage("Greater than duration must be greater than 0.");

        RuleFor(x => x.LessThanDurationMs)
            .Must(x => x == null || x > 0).WithMessage("Less than duration must be greater than 0.");

        RuleFor(x => x.SnapshotId)
            .Must(x => x == null || x != Guid.Empty).WithMessage("Snapshot ID cannot be empty.");

        RuleFor(x => x.SessionId)
            .Must(x => x == null || x != Guid.Empty).WithMessage("Session ID cannot be empty.");

        RuleFor(x => x.CorrelationId)
            .Must(x => x == null || x != Guid.Empty).WithMessage("Correlation ID cannot be empty.");
    }
}