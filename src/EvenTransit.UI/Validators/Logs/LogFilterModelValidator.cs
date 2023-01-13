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
            .WithMessage("Service name cannot be empty");
        //     .NotEmpty()
               .NotEmpty()

        RuleFor(x => x.LogDateFrom)
            .Must(x => DateTime.TryParseExact(x, "dd-MM-yyyy HH:mm", CultureInfo.CurrentCulture, DateTimeStyles.None, out var _))
            .When(x => !string.IsNullOrWhiteSpace(x.LogDateFrom))
            .WithMessage(ValidationConstants.InvalidLogDateFrom);

        RuleFor(x => x.LogDateTo)
            .Must(x => DateTime.TryParseExact(x, "dd-MM-yyyy HH:mm", CultureInfo.CurrentCulture, DateTimeStyles.None, out var _))
            .When(x => !string.IsNullOrWhiteSpace(x.LogDateTo))
            .WithMessage(ValidationConstants.InvalidLogDateTo);
    }
}
