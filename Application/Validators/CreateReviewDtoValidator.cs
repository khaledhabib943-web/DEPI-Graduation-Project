using Application.DTOs;
using FluentValidation;

namespace Application.Validators
{
    public class CreateReviewDtoValidator : AbstractValidator<CreateReviewDto>
    {
        public CreateReviewDtoValidator()
        {
            RuleFor(x => x.Rating)
                .NotEmpty().WithMessage("Rating is required / التقييم مطلوب")
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5 / يجب أن يكون التقييم بين 1 و 5");

            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage("Comment is required / التعليق مطلوب")
                .MinimumLength(5).WithMessage("Comment must be at least 5 characters / يجب أن يتكون التعليق من 5 أحرف على الأقل")
                .MaximumLength(500).WithMessage("Comment cannot exceed 500 characters / لا يمكن أن يتجاوز التعليق 500 حرف");

            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("Customer ID is required / رقم العميل مطلوب")
                .GreaterThan(0).WithMessage("Customer ID must be greater than 0 / رقم العميل يجب أن يكون أكبر من 0");

            RuleFor(x => x.WorkerId)
                .NotEmpty().WithMessage("Worker ID is required / رقم الفني مطلوب")
                .GreaterThan(0).WithMessage("Worker ID must be greater than 0 / رقم الفني يجب أن يكون أكبر من 0");

            RuleFor(x => x.ServiceRequestId)
                .NotEmpty().WithMessage("Service request ID is required / رقم الطلب مطلوب")
                .GreaterThan(0).WithMessage("Service request ID must be greater than 0 / رقم الطلب يجب أن يكون أكبر من 0");
        }
    }
}
