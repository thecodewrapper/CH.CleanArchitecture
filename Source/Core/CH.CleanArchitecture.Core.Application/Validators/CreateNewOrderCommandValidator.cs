using CH.CleanArchitecture.Core.Application.Commands;
using FluentValidation;

namespace CH.CleanArchitecture.Core.Application.Validators
{
    internal class CreateNewOrderCommandValidator : AbstractValidator<CreateNewOrderCommand>
    {
        public CreateNewOrderCommandValidator() {

            RuleFor(x => x.TrackingNumber)
                .NotEmpty().WithMessage("Tracking number must be provided.");
        }
    }
}
