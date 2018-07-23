using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace Web.Features.Account.Account
{
    public class LoginViewModel
    {
        public string Email { get; set; }

        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class LoginViewModelValidator : AbstractValidator<LoginViewModel>
    {
        public LoginViewModelValidator()
        {
            RuleFor(m => m.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(m => m.Password)
                .NotEmpty();
        }
    }
}