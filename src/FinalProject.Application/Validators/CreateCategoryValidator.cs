using FinalProject.Application.DTOs;

namespace FinalProject.Application.Validators
{
    public static class CreateCategoryValidator
    {
        public static ValidationResult Validate(CreateCategoryDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Name))
                errors.Add("Category name is required.");
            else if (dto.Name.Length > 100)
                errors.Add("Category name cannot exceed 100 characters.");

            if (string.IsNullOrWhiteSpace(dto.Description))
                errors.Add("Description is required.");
            else if (dto.Description.Length > 500)
                errors.Add("Description cannot exceed 500 characters.");

            if (string.IsNullOrWhiteSpace(dto.IconUrl))
                errors.Add("Icon URL is required.");

            return errors.Count == 0
                ? ValidationResult.Success()
                : ValidationResult.Failure(errors.ToArray());
        }
    }
}
