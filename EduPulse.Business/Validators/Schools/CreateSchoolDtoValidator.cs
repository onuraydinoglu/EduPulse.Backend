using EduPulse.DTOs.Schools;
using FluentValidation;

namespace EduPulse.Business.Validators.Schools;

public class CreateSchoolDtoValidator : AbstractValidator<CreateSchoolDto>
{
    public CreateSchoolDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Okul adı boş olamaz.")
            .MaximumLength(100).WithMessage("Okul adı en fazla 100 karakter olabilir.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("Şehir boş olamaz.")
            .MaximumLength(50).WithMessage("Şehir en fazla 50 karakter olabilir.");

        RuleFor(x => x.District)
            .NotEmpty().WithMessage("İlçe boş olamaz.")
            .MaximumLength(50).WithMessage("İlçe en fazla 50 karakter olabilir.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Adres boş olamaz.")
            .MaximumLength(250).WithMessage("Adres en fazla 250 karakter olabilir.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Geçerli bir email giriniz.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^0\d{10}$").WithMessage("Telefon numarası 0 ile başlamalı ve 11 haneli olmalıdır.");
    }
}