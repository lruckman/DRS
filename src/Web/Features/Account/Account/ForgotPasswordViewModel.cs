using System.ComponentModel.DataAnnotations;

namespace Web.Features.Account.Account
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
