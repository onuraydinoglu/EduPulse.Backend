using EduPulse.DTOs.Lessons;
using FluentValidation;

namespace EduPulse.Business.Validators.Lessons;

public class UpdateLessonDtoValidator : AbstractValidator<UpdateLessonDto>
{
    public UpdateLessonDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Ders Id boş olamaz.");

        RuleFor(x => x.SchoolId)
            .NotEmpty().WithMessage("Okul Id boş olamaz.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ders adı boş olamaz.")
            .MaximumLength(50).WithMessage("Ders adı en fazla 50 karakter olabilir.");
    }
}