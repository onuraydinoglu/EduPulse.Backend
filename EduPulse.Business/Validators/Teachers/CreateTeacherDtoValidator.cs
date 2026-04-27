using EduPulse.DTOs.Teachers;
using FluentValidation;

namespace EduPulse.Business.Validators.Teachers;

public class CreateTeacherDtoValidator : AbstractValidator<CreateTeacherDto>
{
    public CreateTeacherDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Öğretmen adı boş olamaz.")
            .MaximumLength(50).WithMessage("Öğretmen adı en fazla 50 karakter olabilir.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Öğretmen soyadı boş olamaz.")
            .MaximumLength(50).WithMessage("Öğretmen soyadı en fazla 50 karakter olabilir.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Telefon numarası boş olamaz.")
            .Matches(@"^0\d{10}$").WithMessage("Telefon numarası 0 ile başlamalı ve 11 haneli olmalıdır.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Geçerli bir email giriniz.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre boş olamaz.")
            .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.")
            .MaximumLength(50).WithMessage("Şifre en fazla 50 karakter olmalıdır.");

        RuleFor(x => x.SchoolId)
            .NotEmpty().WithMessage("Okul Id boş olamaz.");
    }
}