using FinalProject.Application.DTOs;

namespace FinalProject.Application.Validators
{
    public static class CreateServiceRequestValidator
    {
        public static ValidationResult Validate(CreateServiceRequestDto dto)
        {
            var errors = new List<string>();

            if (dto.WorkerId <= 0)
                errors.Add("A valid worker must be selected.");

            if (dto.CategoryId <= 0)
                errors.Add("A valid category must be selected.");

            if (string.IsNullOrWhiteSpace(dto.LocationDetails))
                errors.Add("Location details are required.");

            if (dto.ScheduledDate < DateTime.Today)
                errors.Add("Scheduled date cannot be in the past.");

            if (string.IsNullOrWhiteSpace(dto.Description))
                errors.Add("Description is required.");
            else if (dto.Description.Length > 1000)
                errors.Add("Description cannot exceed 1000 characters.");

            return errors.Count == 0
                ? ValidationResult.Success()
                : ValidationResult.Failure(errors.ToArray());
        }
    }
}
