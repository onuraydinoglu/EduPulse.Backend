using EduPulse.DTOs.StudentGrades;
using FluentValidation;

namespace EduPulse.Business.Validators.StudentGrades;

public class CreateStudentGradeDtoValidator : AbstractValidator<CreateStudentGradeDto>
{
    public CreateStudentGradeDtoValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.LessonId).NotEmpty();

        RuleFor(x => x.Exam1).InclusiveBetween(0, 100);
        RuleFor(x => x.Exam2).InclusiveBetween(0, 100);
        RuleFor(x => x.Project).InclusiveBetween(0, 100);

        RuleFor(x => x.Activity1).InclusiveBetween(0, 100);
        RuleFor(x => x.Activity2).InclusiveBetween(0, 100);
        RuleFor(x => x.Activity3).InclusiveBetween(0, 100);
    }
}