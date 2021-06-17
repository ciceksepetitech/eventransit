using EvenTransit.Api.Models;
using EvenTransit.Core.Constants;
using FluentValidation;

namespace EvenTransit.Api.Validators
{
    public class QueueRequestValidator : AbstractValidator<EventRequest>
    {
        public QueueRequestValidator()
        {
            RuleFor(x => x.EventName)
                .Must(x => !string.IsNullOrEmpty(x))
                .WithMessage(string.Format(ValidationConstants.IsRequired, "EventName"));
        }
    }
}