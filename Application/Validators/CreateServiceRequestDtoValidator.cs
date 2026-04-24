using Application.DTOs;
using FluentValidation;

namespace Application.Validators
{
    public class CreateServiceRequestDtoValidator : AbstractValidator<CreateServiceRequestDto>
    {
        public CreateServiceRequestDtoValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required / الوصف مطلوب")
                .MinimumLength(10).WithMessage("Description must be at least 10 characters / يجب أن يتكون الوصف من 10 أحرف على الأقل")
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters / لا يمكن أن يتجاوز الوصف 500 حرف");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required / العنوان مطلوب")
                .MinimumLength(5).WithMessage("Address must be at least 5 characters / يجب أن يتكون العنوان من 5 أحرف على الأقل")
                .MaximumLength(200).WithMessage("Address cannot exceed 200 characters / لا يمكن أن يتجاوز العنوان 200 حرف");

            RuleFor(x => x.ScheduledDate)
                .NotEmpty().WithMessage("Scheduled date is required / الموعد مطلوب")
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Scheduled date must be in the future / يجب أن يكون الموعد في المستقبل أو اليوم");

            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("Customer ID is required / رقم العميل مطلوب")
                .GreaterThan(0).WithMessage("Customer ID must be greater than 0 / رقم العميل يجب أن يكون أكبر من 0");

            RuleFor(x => x.WorkerId)
                .NotEmpty().WithMessage("Worker ID is required / رقم الفني مطلوب")
                .GreaterThan(0).WithMessage("Worker ID must be greater than 0 / رقم الفني يجب أن يكون أكبر من 0");
        }
    }
}
