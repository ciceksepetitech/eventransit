using System;
using EvenTransit.Domain.Constants;
using EvenTransit.UI.Models.Events;
using FluentValidation;

namespace EvenTransit.UI.Validators.Events
{
    public class SaveServiceModelValidator : AbstractValidator<SaveServiceModel>
    {
        public SaveServiceModelValidator()
        {
            RuleFor(x => x.EventId)
                .Must(x => x != Guid.Empty)
                .WithMessage("Event id cannot be empty");

            RuleFor(x => x.ServiceName)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage("Service name cannot be empty");

            RuleFor(x => x.Url)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage("Url cannot be empty");
            
            RuleFor(x => x.ServiceName)
                .Matches(@"^([a-zA-Z\-])+$")
                .WithMessage(ValidationConstants.ServiceNameRegexError);

            RuleFor(x => x.Timeout)
                .GreaterThanOrEqualTo(0)
                .WithMessage(ValidationConstants.TimeoutMustBeGreaterThanZero);

            RuleFor(x => x.Method)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage(ValidationConstants.MethodCannotBeNotEmpty);
        }
    }
}