using Application.DTOs;
using FluentValidation;

namespace Application.Validators
{
    public class CreateServiceRequestDtoValidator : AbstractValidator<CreateServiceRequestDto>
    {
        public CreateServiceRequestDtoValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MinimumLength(10).WithMessage("Description must be at least 10 characters")
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required")
                .MinimumLength(5).WithMessage("Address must be at least 5 characters")
                .MaximumLength(200).WithMessage("Address cannot exceed 200 characters");

            RuleFor(x => x.ScheduledDate)
                .NotEmpty().WithMessage("Scheduled date is required")
                .Must(date => date > DateTime.Now).WithMessage("Scheduled date must be in the future");

            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("Customer ID is required")
                .GreaterThan(0).WithMessage("Customer ID must be greater than 0");

            RuleFor(x => x.WorkerId)
                .NotEmpty().WithMessage("Worker ID is required")
                .GreaterThan(0).WithMessage("Worker ID must be greater than 0");
        }
    }
}
