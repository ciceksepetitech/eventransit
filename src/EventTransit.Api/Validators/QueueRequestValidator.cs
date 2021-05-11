using EventTransit.Api.Models;
using EventTransit.Core.Constants;
using FluentValidation;

namespace EventTransit.Api.Validators
{
    public class QueueRequestValidator : AbstractValidator<QueueRequest>
    {
        public QueueRequestValidator()
        {
            RuleFor(x => x.Name)
                .Must(x => !string.IsNullOrEmpty(x))
                .WithMessage(string.Format(ValidationConstants.IsRequired, "Name"));
        }
    }
}