using Application.DTOs;
using FluentValidation;

namespace Application.Validators
{
    public class CreateReviewDtoValidator : AbstractValidator<CreateReviewDto>
    {
        public CreateReviewDtoValidator()
        {
            RuleFor(x => x.Rating)
                .NotEmpty().WithMessage("Rating is required")
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5");

            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage("Comment is required")
                .MinimumLength(5).WithMessage("Comment must be at least 5 characters")
                .MaximumLength(500).WithMessage("Comment cannot exceed 500 characters");

            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("Customer ID is required")
                .GreaterThan(0).WithMessage("Customer ID must be greater than 0");

            RuleFor(x => x.WorkerId)
                .NotEmpty().WithMessage("Worker ID is required")
                .GreaterThan(0).WithMessage("Worker ID must be greater than 0");

            RuleFor(x => x.ServiceRequestId)
                .NotEmpty().WithMessage("Service request ID is required")
                .GreaterThan(0).WithMessage("Service request ID must be greater than 0");
        }
    }
}
