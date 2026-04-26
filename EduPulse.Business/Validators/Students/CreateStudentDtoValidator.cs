using EduPulse.DTOs.Students;
using FluentValidation;

namespace EduPulse.Business.Validators.Students;

public class CreateStudentDtoValidator : AbstractValidator<CreateStudentDto>
{
    public CreateStudentDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Öğrenci adı boş olamaz.")
            .MaximumLength(50);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Öğrenci soyadı boş olamaz.")
            .MaximumLength(50);

        RuleFor(x => x.SchoolNumber)
            .NotEmpty().WithMessage("Okul numarası boş olamaz.")
            .MaximumLength(20);

        RuleFor(x => x.StudentPhone)
            .NotEmpty().WithMessage("Telefon numarası boş olamaz.")
            .Matches(@"^05\d{9}$").WithMessage("Telefon numarası 05 ile başlamalı ve 11 haneli olmalıdır.");

        RuleFor(x => x.SchoolId)
            .NotEmpty().WithMessage("Okul Id boş olamaz.");

        RuleFor(x => x.ClassroomId)
            .NotEmpty().WithMessage("Sınıf Id boş olamaz.");
    }
}