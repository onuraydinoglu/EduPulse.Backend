using EduPulse.DTOs.Auth;
using FluentValidation;

namespace EduPulse.Business.Validators.Auth;

public class RegisterSchoolDtoValidator : AbstractValidator<RegisterSchoolDto>
{
    public RegisterSchoolDtoValidator()
    {
        RuleFor(x => x.SchoolName)
            .NotEmpty().WithMessage("Okul adı boş olamaz.")
            .MinimumLength(2).WithMessage("Okul adı en az 2 karakter olmalıdır.")
            .MaximumLength(100).WithMessage("Okul adı en fazla 100 karakter olmalıdır.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("Şehir boş olamaz.")
            .MaximumLength(50).WithMessage("Şehir en fazla 50 karakter olmalıdır.");

        RuleFor(x => x.District)
            .NotEmpty().WithMessage("İlçe boş olamaz.")
            .MaximumLength(50).WithMessage("İlçe en fazla 50 karakter olmalıdır.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Adres boş olamaz.")
            .MaximumLength(250).WithMessage("Adres en fazla 250 karakter olmalıdır.");

        RuleFor(x => x.SchoolPhoneNumber)
            .NotEmpty().WithMessage("Okul telefon numarası boş olamaz.")
            .MaximumLength(20).WithMessage("Okul telefon numarası en fazla 20 karakter olmalıdır.");

        RuleFor(x => x.SchoolEmail)
            .NotEmpty().WithMessage("Okul e-posta adresi boş olamaz.")
            .EmailAddress().WithMessage("Geçerli bir okul e-posta adresi giriniz.")
            .MaximumLength(100).WithMessage("Okul e-posta adresi en fazla 100 karakter olmalıdır.");

        RuleFor(x => x.AdminFullName)
            .NotEmpty().WithMessage("Yetkili adı soyadı boş olamaz.")
            .MinimumLength(2).WithMessage("Yetkili adı soyadı en az 2 karakter olmalıdır.")
            .MaximumLength(100).WithMessage("Yetkili adı soyadı en fazla 100 karakter olmalıdır.");

        RuleFor(x => x.AdminEmail)
            .NotEmpty().WithMessage("Yetkili e-posta adresi boş olamaz.")
            .EmailAddress().WithMessage("Geçerli bir yetkili e-posta adresi giriniz.")
            .MaximumLength(100).WithMessage("Yetkili e-posta adresi en fazla 100 karakter olmalıdır.");

        RuleFor(x => x.AdminPassword)
            .NotEmpty().WithMessage("Şifre boş olamaz.")
            .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.")
            .MaximumLength(50).WithMessage("Şifre en fazla 50 karakter olmalıdır.");
    }
}