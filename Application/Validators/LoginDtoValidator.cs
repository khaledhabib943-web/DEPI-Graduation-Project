using Application.DTOs;
using FluentValidation;

namespace Application.Validators
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required / البريد الإلكتروني مطلوب")
                .EmailAddress().WithMessage("Invalid email format / صيغة البريد الإلكتروني غير صالحة");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required / كلمة المرور مطلوبة");
        }
    }
}
