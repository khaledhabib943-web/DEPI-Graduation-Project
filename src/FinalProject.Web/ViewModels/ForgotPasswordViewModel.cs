using System.ComponentModel.DataAnnotations;

namespace FinalProject.Web.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }
    }
}
