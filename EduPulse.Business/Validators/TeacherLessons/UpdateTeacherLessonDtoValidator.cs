using EduPulse.DTOs.TeacherLessons;
using FluentValidation;

namespace EduPulse.Business.Validators.TeacherLessons;

public class UpdateTeacherLessonDtoValidator : AbstractValidator<UpdateTeacherLessonDto>
{
    public UpdateTeacherLessonDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Kayıt Id boş olamaz.");

        RuleFor(x => x.SchoolId)
            .NotEmpty().WithMessage("Okul seçilmelidir.");

        RuleFor(x => x.TeacherId)
            .NotEmpty().WithMessage("Öğretmen seçilmelidir.");

        RuleFor(x => x.LessonId)
            .NotEmpty().WithMessage("Ders seçilmelidir.");

        RuleFor(x => x.ClassroomId)
            .NotEmpty().WithMessage("Sınıf seçilmelidir.");
    }
}