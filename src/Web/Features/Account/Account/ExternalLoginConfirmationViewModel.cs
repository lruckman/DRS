using System.ComponentModel.DataAnnotations;

namespace Web.Features.Account.Account
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
