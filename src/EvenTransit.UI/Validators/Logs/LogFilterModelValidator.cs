using System;
using EvenTransit.Domain.Constants;
using EvenTransit.UI.Models.Logs;
using FluentValidation;

namespace EvenTransit.UI.Validators.Logs
{
    public class LogFilterModelValidator : AbstractValidator<LogFilterModel>
    {
        public LogFilterModelValidator()
        {
            RuleFor(x => x.EventName)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage("Event name cannot be empty");

            RuleFor(x => x.ServiceName)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage("Service name cannot be empty");

            RuleFor(x => x.LogDateFrom)
                .Must(x => DateTime.TryParse(x, out var _))
                .WithMessage(ValidationConstants.InvalidLogDateFrom);
            
            RuleFor(x => x.LogDateTo)
                .Must(x => DateTime.TryParse(x, out var _))
                .WithMessage(ValidationConstants.InvalidLogDateTo);
        }
    }
}