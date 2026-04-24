using Application.DTOs;
using FluentValidation;

namespace Application.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required / الاسم بالكامل مطلوب")
                .MinimumLength(3).WithMessage("Full name must be at least 3 characters / يجب أن يتكون الاسم من 3 أحرف على الأقل")
                .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters / لا يمكن أن يتجاوز الاسم 100 حرف");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required / البريد الإلكتروني مطلوب")
                .EmailAddress().WithMessage("Invalid email format / صيغة البريد الإلكتروني غير صالحة");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required / كلمة المرور مطلوبة")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters / يجب أن تتكون كلمة المرور من 8 أحرف على الأقل")
                .Matches(@"[A-Z]+").WithMessage("Password must contain at least one uppercase letter / يجب أن تحتوي كلمة المرور على حرف كبير واحد على الأقل")
                .Matches(@"[a-z]+").WithMessage("Password must contain at least one lowercase letter / يجب أن تحتوي كلمة المرور على حرف صغير واحد على الأقل")
                .Matches(@"[0-9]+").WithMessage("Password must contain at least one number / يجب أن تحتوي كلمة المرور على رقم واحد على الأقل");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required / رقم الهاتف مطلوب")
                .Matches(@"^01[0125][0-9]{8}$").WithMessage("Invalid Egyptian phone number format / صيغة رقم الهاتف المصري غير صالحة");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role is required / نوع الحساب مطلوب")
                .Must(role => role == "Customer" || role == "Worker").WithMessage("Role must be either 'Customer' or 'Worker' / نوع الحساب يجب أن يكون عميل أو فني");
        }
    }
}
