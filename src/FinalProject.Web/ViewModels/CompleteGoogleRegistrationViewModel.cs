using System.ComponentModel.DataAnnotations;
using FinalProject.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace FinalProject.Web.ViewModels
{
    public class CompleteGoogleRegistrationViewModel
    {
        // Pre-filled from Google (read-only on the form)
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        // User must fill these
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3-50 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9_\.]+$", ErrorMessage = "Username can only contain letters, numbers, underscores, and dots.")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(128, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm your password.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^01[0125]\d{8}$", ErrorMessage = "Please enter a valid Egyptian phone number (e.g., 01012345678).")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "National ID is required.")]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "National ID must be exactly 14 digits.")]
        [Display(Name = "National ID")]
        public string NationalId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Age is required.")]
        [Range(18, 100, ErrorMessage = "Age must be between 18 and 100.")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(300, MinimumLength = 10, ErrorMessage = "Address must be between 10-300 characters.")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role selection is required.")]
        public UserRole Role { get; set; } = UserRole.Customer;

        // Worker specific fields
        [Display(Name = "Category")]
        public int? CategoryId { get; set; }

        [Display(Name = "Price per hour (EGP)")]
        [Range(1, 100000, ErrorMessage = "Price must be greater than zero.")]
        public decimal? ServicePrice { get; set; }

        [Display(Name = "National ID Front (Image)")]
        public IFormFile? IdFrontFile { get; set; }

        [Display(Name = "National ID Back (Image)")]
        public IFormFile? IdBackFile { get; set; }

        [Display(Name = "Portfolio Document (Optional)")]
        public IFormFile? PortfolioFile { get; set; }

        [Display(Name = "Profile Picture (Optional)")]
        public IFormFile? ProfilePictureFile { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
