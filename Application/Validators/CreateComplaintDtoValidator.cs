using Application.DTOs;
using FluentValidation;

namespace Application.Validators
{
    public class CreateComplaintDtoValidator : AbstractValidator<CreateComplaintDto>
    {
        public CreateComplaintDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MinimumLength(5).WithMessage("Title must be at least 5 characters")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required")
                .MinimumLength(20).WithMessage("Content must be at least 20 characters")
                .MaximumLength(1000).WithMessage("Content cannot exceed 1000 characters");

            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("Customer ID is required")
                .GreaterThan(0).WithMessage("Customer ID must be greater than 0");

            RuleFor(x => x.ServiceRequestId)
                .GreaterThan(0).WithMessage("Service request ID must be greater than 0")
                .When(x => x.ServiceRequestId.HasValue);
        }
    }
}
