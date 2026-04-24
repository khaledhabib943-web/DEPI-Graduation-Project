using Application.DTOs;
using FluentValidation;

namespace Application.Validators
{
    public class CreateComplaintDtoValidator : AbstractValidator<CreateComplaintDto>
    {
        public CreateComplaintDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required / عنوان الشكوى مطلوب")
                .MinimumLength(5).WithMessage("Title must be at least 5 characters / يجب أن يتكون العنوان من 5 أحرف على الأقل")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters / لا يمكن أن يتجاوز العنوان 100 حرف");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required / محتوى الشكوى مطلوب")
                .MinimumLength(20).WithMessage("Content must be at least 20 characters / يجب أن يتكون المحتوى من 20 حرفًا على الأقل")
                .MaximumLength(1000).WithMessage("Content cannot exceed 1000 characters / لا يمكن أن يتجاوز المحتوى 1000 حرف");

            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("Customer ID is required / رقم العميل مطلوب")
                .GreaterThan(0).WithMessage("Customer ID must be greater than 0 / رقم العميل يجب أن يكون أكبر من 0");

            RuleFor(x => x.ServiceRequestId)
                .GreaterThan(0).WithMessage("Service request ID must be greater than 0 / رقم الطلب يجب أن يكون أكبر من 0")
                .When(x => x.ServiceRequestId.HasValue);
        }
    }
}
