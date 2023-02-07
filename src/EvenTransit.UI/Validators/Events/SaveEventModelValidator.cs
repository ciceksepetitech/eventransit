using EvenTransit.Domain.Constants;
using EvenTransit.UI.Models.Events;
using FluentValidation;

namespace EvenTransit.UI.Validators.Events;

public class SaveEventModelValidator : AbstractValidator<SaveEventModel>
{
    public SaveEventModelValidator()
    {
        RuleFor(x => x.EventName)
            .NotEmpty()
            .WithMessage("Event name cannot be empty");

        RuleFor(x => x.EventName)
            .Matches(@"^([a-zA-Z\-])+$")
            .WithMessage(ValidationConstants.EventNameRegexError);
    }
}
