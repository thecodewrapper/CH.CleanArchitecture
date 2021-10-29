using CH.CleanArchitecture.Presentation.Web.ViewModels;
using FluentValidation;

namespace CH.CleanArchitecture.Presentation.Web.Validators
{
    public partial class LoginValidator : AbstractValidator<LoginViewModel>
    {
        public LoginValidator() {
            RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
        }
    }
}
