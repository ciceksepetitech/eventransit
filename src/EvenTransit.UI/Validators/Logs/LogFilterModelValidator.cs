using EvenTransit.UI.Models.Logs;
using FluentValidation;

namespace EvenTransit.UI.Validation.Logs
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
        }
    }
}