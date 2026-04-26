using EduPulse.DTOs.TeacherLessons;
using FluentValidation;

namespace EduPulse.Business.Validators.TeacherLessons;

public class CreateTeacherLessonDtoValidator : AbstractValidator<CreateTeacherLessonDto>
{
    public CreateTeacherLessonDtoValidator()
    {
        RuleFor(x => x.SchoolId).NotEmpty();
        RuleFor(x => x.TeacherId).NotEmpty();
        RuleFor(x => x.LessonId).NotEmpty();
        RuleFor(x => x.ClassroomId).NotEmpty();
    }
}