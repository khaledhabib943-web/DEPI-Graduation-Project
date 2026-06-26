using FinalProject.Application.DTOs;

namespace FinalProject.Application.Validators
{
    public static class CreateComplaintValidator
    {
        public static ValidationResult Validate(CreateComplaintDto dto)
        {
            var errors = new List<string>();

            if (dto.WorkerId <= 0)
                errors.Add("A valid worker must be specified.");

            if (string.IsNullOrWhiteSpace(dto.Description))
                errors.Add("Complaint description is required.");
            else if (dto.Description.Length > 2000)
                errors.Add("Description cannot exceed 2000 characters.");

            return errors.Count == 0
                ? ValidationResult.Success()
                : ValidationResult.Failure(errors.ToArray());
        }
    }
}
