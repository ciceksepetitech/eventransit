using EvenTransit.Domain.Constants;
using EvenTransit.UI.Models.Api;
using FluentValidation;

namespace EvenTransit.UI.Validators.Api;

public class EventRequestValidator : AbstractValidator<EventRequest>
{
    public EventRequestValidator()
    {
        RuleFor(x => x.EventName)
            .Must(x => !string.IsNullOrEmpty(x))
            .WithMessage(string.Format(ValidationConstants.IsRequired, "EventName"));
    }
}
