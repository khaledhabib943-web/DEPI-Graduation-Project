using FinalProject.Application.DTOs;

namespace FinalProject.Application.Validators
{
    public static class CreateReviewValidator
    {
        public static ValidationResult Validate(CreateReviewDto dto)
        {
            var errors = new List<string>();

            if (dto.WorkerId <= 0)
                errors.Add("A valid worker must be specified.");

            if (dto.RequestId <= 0)
                errors.Add("A valid service request must be specified.");

            if (dto.Rating < 1 || dto.Rating > 5)
                errors.Add("Rating must be between 1 and 5.");

            if (string.IsNullOrWhiteSpace(dto.Comment))
                errors.Add("Comment is required.");
            else if (dto.Comment.Length > 1000)
                errors.Add("Comment cannot exceed 1000 characters.");

            return errors.Count == 0
                ? ValidationResult.Success()
                : ValidationResult.Failure(errors.ToArray());
        }
    }
}
