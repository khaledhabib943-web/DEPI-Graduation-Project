using System.ComponentModel.DataAnnotations;

namespace FinalProject.Web.ViewModels
{
    /// <summary>
    /// Used when a user is setting up 2FA from their profile.
    /// Carries the shared key and QR URI for the authenticator app.
    /// </summary>
    public class Setup2faViewModel
    {
        public string SharedKey { get; set; } = string.Empty;
        public string AuthenticatorUri { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the 6-digit code from your authenticator app.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Code must be exactly 6 digits.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Code must be exactly 6 digits.")]
        [Display(Name = "Verification Code")]
        public string Code { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Used during login when a user has 2FA enabled.
    /// Collects the 6-digit TOTP code.
    /// </summary>
    public class Verify2faViewModel
    {
        [Required(ErrorMessage = "Please enter the 6-digit code from your authenticator app.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Code must be exactly 6 digits.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Code must be exactly 6 digits.")]
        [Display(Name = "Verification Code")]
        public string Code { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Displays one-time recovery codes after enabling 2FA.
    /// </summary>
    public class RecoveryCodesViewModel
    {
        public IEnumerable<string> RecoveryCodes { get; set; } = Array.Empty<string>();
    }
}
