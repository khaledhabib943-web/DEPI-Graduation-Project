using FinalProject.Application.DTOs;

namespace FinalProject.Application.Validators
{
    public static class RegisterCustomerValidator
    {
        public static ValidationResult Validate(RegisterCustomerDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.FullName))
                errors.Add("Full name is required.");
            else if (dto.FullName.Length > 100)
                errors.Add("Full name cannot exceed 100 characters.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                errors.Add("Email is required.");
            else if (!dto.Email.Contains('@'))
                errors.Add("Email format is invalid.");

            if (string.IsNullOrWhiteSpace(dto.Password))
                errors.Add("Password is required.");
            else if (dto.Password.Length < 6)
                errors.Add("Password must be at least 6 characters.");

            if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
                errors.Add("Phone number is required.");

            if (string.IsNullOrWhiteSpace(dto.NationalId))
                errors.Add("National ID is required.");

            if (dto.Age < 18 || dto.Age > 120)
                errors.Add("Age must be between 18 and 120.");

            if (string.IsNullOrWhiteSpace(dto.Username))
                errors.Add("Username is required.");
            else if (dto.Username.Length < 3 || dto.Username.Length > 50)
                errors.Add("Username must be between 3 and 50 characters.");

            if (string.IsNullOrWhiteSpace(dto.Address))
                errors.Add("Address is required.");

            return errors.Count == 0
                ? ValidationResult.Success()
                : ValidationResult.Failure(errors.ToArray());
        }
    }
}
