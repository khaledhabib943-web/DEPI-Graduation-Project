using FinalProject.Application.DTOs;

namespace FinalProject.Application.Validators
{
    public static class RegisterWorkerValidator
    {
        public static ValidationResult Validate(RegisterWorkerDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.FullName))
                errors.Add("Full name is required.");

            if (string.IsNullOrWhiteSpace(dto.Email) || !dto.Email.Contains('@'))
                errors.Add("A valid email is required.");

            // ── Strengthened password rules ──
            if (string.IsNullOrWhiteSpace(dto.Password))
                errors.Add("Password is required.");
            else
            {
                if (dto.Password.Length < 8)
                    errors.Add("Password must be at least 8 characters.");
                if (!dto.Password.Any(char.IsUpper))
                    errors.Add("Password must contain at least one uppercase letter.");
                if (!dto.Password.Any(char.IsLower))
                    errors.Add("Password must contain at least one lowercase letter.");
                if (!dto.Password.Any(char.IsDigit))
                    errors.Add("Password must contain at least one digit.");
                if (!dto.Password.Any(c => !char.IsLetterOrDigit(c)))
                    errors.Add("Password must contain at least one special character.");
                if (dto.Password.Distinct().Count() < 4)
                    errors.Add("Password must contain at least 4 unique characters.");
            }

            if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
                errors.Add("Phone number is required.");

            if (string.IsNullOrWhiteSpace(dto.NationalId))
                errors.Add("National ID is required.");

            if (dto.Age < 18 || dto.Age > 120)
                errors.Add("Age must be between 18 and 120.");

            if (string.IsNullOrWhiteSpace(dto.Username) || dto.Username.Length < 3)
                errors.Add("Username must be at least 3 characters.");

            if (dto.CategoryId <= 0)
                errors.Add("A valid category must be selected.");

            if (string.IsNullOrWhiteSpace(dto.ProfilePicture))
                errors.Add("Profile picture URL is required.");

            if (string.IsNullOrWhiteSpace(dto.IdFrontImage))
                errors.Add("ID front image URL is required.");

            if (string.IsNullOrWhiteSpace(dto.IdBackImage))
                errors.Add("ID back image URL is required.");

            if (dto.ServicePrice <= 0)
                errors.Add("Service price must be greater than zero.");

            return errors.Count == 0
                ? ValidationResult.Success()
                : ValidationResult.Failure(errors.ToArray());
        }
    }
}