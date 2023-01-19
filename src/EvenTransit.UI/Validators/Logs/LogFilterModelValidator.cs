using System;
using EvenTransit.Domain.Constants;
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
            .Must(x => DateTime.TryParseExact(x, "dd-MM-yyyy HH:mm", CultureInfo.CurrentCulture, DateTimeStyles.None, out var _))
            .When(x => !string.IsNullOrWhiteSpace(x.LogDateFrom))
            .WithMessage(ValidationConstants.InvalidLogDateFrom);

        RuleFor(x => x.LogDateTo)
            .NotEmpty();

        RuleFor(x => x.LogDateTo)
            .Must(x => DateTime.TryParseExact(x, "dd-MM-yyyy HH:mm", CultureInfo.CurrentCulture, DateTimeStyles.None, out var _))
            .When(x => !string.IsNullOrWhiteSpace(x.LogDateTo))
            .WithMessage(ValidationConstants.InvalidLogDateTo);

        RuleFor(w => w)
            .Must(w =>
            {
                DateTime.TryParse(w.LogDateFrom, out var startDate);
                DateTime.TryParse(w.LogDateTo, out var endDate);
                return (endDate - startDate).TotalHours <= 24;
            }).WithMessage("Date range must be max 24 hours when 'Query' string provided")
            .When(w => !string.IsNullOrWhiteSpace(w.Query));
    }
}
