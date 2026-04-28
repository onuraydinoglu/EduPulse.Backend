using EduPulse.DTOs.Auth;
using FluentValidation;

namespace EduPulse.Business.Validators.Auth;

public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Ad boş olamaz.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Soyad boş olamaz.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email boş olamaz.")
            .EmailAddress().WithMessage("Geçerli bir email giriniz.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre boş olamaz.")
            .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalı.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Telefon boş olamaz.");

        RuleFor(x => x.SchoolCode)
            .NotEmpty().WithMessage("Okul kodu boş olamaz.");
    }
}