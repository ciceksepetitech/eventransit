using EvenTransit.Api.Models;
using EvenTransit.Domain.Constants;
using FluentValidation;

namespace EvenTransit.Api.Validators
{
    public class EventRequestValidator : AbstractValidator<EventRequest>
    {
        public EventRequestValidator()
        {
            RuleFor(x => x.EventName)
                .Must(x => !string.IsNullOrEmpty(x))
                .WithMessage(string.Format(ValidationConstants.IsRequired, "EventName"));
        }
    }
}