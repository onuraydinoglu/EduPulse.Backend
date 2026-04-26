using EduPulse.DTOs.Lessons;
using FluentValidation;

namespace EduPulse.Business.Validators.Lessons;

public class CreateLessonDtoValidator : AbstractValidator<CreateLessonDto>
{
    public CreateLessonDtoValidator()
    {
        RuleFor(x => x.SchoolId)
            .NotEmpty().WithMessage("Okul Id boş olamaz.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ders adı boş olamaz.")
            .MaximumLength(50).WithMessage("Ders adı en fazla 50 karakter olabilir.");
    }
}