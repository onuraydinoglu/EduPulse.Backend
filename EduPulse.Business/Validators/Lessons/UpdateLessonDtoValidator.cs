using EduPulse.DTOs.Lessons;
using FluentValidation;

namespace EduPulse.Business.Validators.Lessons;

public class UpdateLessonDtoValidator : AbstractValidator<UpdateLessonDto>
{
    public UpdateLessonDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Ders Id boş olamaz.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ders adı boş olamaz.")
            .MinimumLength(2).WithMessage("Ders adı en az 2 karakter olmalıdır.")
            .MaximumLength(100).WithMessage("Ders adı en fazla 100 karakter olabilir.");
    }
}