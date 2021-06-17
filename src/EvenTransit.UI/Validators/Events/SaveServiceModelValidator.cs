using System;
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
        }
    }
}