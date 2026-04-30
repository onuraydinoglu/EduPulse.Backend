using EduPulse.DTOs.TeacherLessons;
using FluentValidation;

namespace EduPulse.Business.Validators.TeacherLessons;

public class CreateTeacherLessonDtoValidator : AbstractValidator<CreateTeacherLessonDto>
{
    public CreateTeacherLessonDtoValidator()
    {
        RuleFor(x => x.TeacherId)
            .NotEmpty().WithMessage("Öğretmen seçilmelidir.");

        RuleFor(x => x.LessonId)
            .NotEmpty().WithMessage("Ders seçilmelidir.");

        RuleFor(x => x.ClassroomId)
            .NotEmpty().WithMessage("Sınıf seçilmelidir.");
    }
}