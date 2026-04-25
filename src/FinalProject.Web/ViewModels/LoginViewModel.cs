using System.ComponentModel.DataAnnotations;

namespace FinalProject.Web.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username or email is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Must be between 3-100 characters.")]
        [Display(Name = "Username or Email")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(128, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
