using EvenTransit.Domain.Constants;
using EvenTransit.Domain.Extensions;
using EvenTransit.UI.Models.Logs;
using FluentValidation;
using System.Globalization;

namespace EvenTransit.UI.Validators.Logs;

public class LogFilterModelValidator : AbstractValidator<LogFilterModel>
{
    public LogFilterModelValidator()
    {
        RuleFor(x => x.EventName)
            .NotEmpty()
            .WithMessage("Event name cannot be empty");

        RuleFor(x => x.ServiceName)
            .NotEmpty()
            .WithMessage("Service name cannot be empty");

        RuleFor(x => x.LogDateFrom)
            .NotEmpty();

        RuleFor(x => x.LogDateFrom)
            .Must(x => x.TryConvertToDate(out var _))
            .When(x => !string.IsNullOrWhiteSpace(x.LogDateFrom))
            .WithMessage(ValidationConstants.InvalidLogDateFrom);

        RuleFor(x => x.LogDateTo)
            .NotEmpty();

        RuleFor(x => x.LogDateTo)
            .Must(x => x.TryConvertToDate(out var _))
            .When(x => !string.IsNullOrWhiteSpace(x.LogDateTo))
            .WithMessage(ValidationConstants.InvalidLogDateTo);

        const int maxHourRange = 4;
        RuleFor(w => w)
            .Must(w =>
            {
                var success = w.LogDateFrom.TryConvertToDate(out var startDate);
                success &= w.LogDateTo.TryConvertToDate(out var endDate);
                return success && (endDate - startDate).TotalHours <= maxHourRange;
            }).WithMessage($"Date range must be max {maxHourRange} hours when 'Query' string provided")
            .When(w => !string.IsNullOrWhiteSpace(w.Query));
    }
}
