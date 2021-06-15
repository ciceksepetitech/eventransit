using EvenTransit.UI.Models.Events;
using FluentValidation;

namespace EvenTransit.UI.Validators.Events
{
    public class SaveEventModelValidator :  AbstractValidator<SaveEventModel>
    {
        public SaveEventModelValidator()
        {
            RuleFor(x => x.EventName)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage("Event name cannot be empty");
        }
    }
}