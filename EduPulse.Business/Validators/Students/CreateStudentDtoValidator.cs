using EduPulse.DTOs.Students;
using FluentValidation;

namespace EduPulse.Business.Validators.Students;

public class CreateStudentDtoValidator : AbstractValidator<CreateStudentDto>
{
    public CreateStudentDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Ad boş olamaz.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Soyad boş olamaz.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^0\d{10}$").WithMessage("Telefon 0 ile başlamalı ve 11 haneli olmalıdır.")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleFor(x => x.ClassroomId)
            .NotEmpty().WithMessage("Sınıf seçilmelidir.");

        RuleFor(x => x.StudentNumber)
            .NotEmpty().WithMessage("Öğrenci numarası boş olamaz.");
    }
}