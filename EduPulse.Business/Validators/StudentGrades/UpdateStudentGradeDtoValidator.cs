using EduPulse.DTOs.StudentGrades;
using FluentValidation;

namespace EduPulse.Business.Validators.StudentGrades;

public class UpdateStudentGradeDtoValidator : AbstractValidator<UpdateStudentGradeDto>
{
    public UpdateStudentGradeDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Not Id boş olamaz.");

        RuleFor(x => x.SchoolId)
            .NotEmpty()
            .WithMessage("Okul seçilmelidir.");

        RuleFor(x => x.TeacherId)
            .NotEmpty()
            .WithMessage("Öğretmen seçilmelidir.");

        RuleFor(x => x.StudentId)
            .NotEmpty()
            .WithMessage("Öğrenci seçilmelidir.");

        RuleFor(x => x.LessonId)
            .NotEmpty()
            .WithMessage("Ders seçilmelidir.");

        RuleFor(x => x.Exam1).InclusiveBetween(0, 100);
        RuleFor(x => x.Exam2).InclusiveBetween(0, 100);
        RuleFor(x => x.Project).InclusiveBetween(0, 100);
        RuleFor(x => x.Activity1).InclusiveBetween(0, 100);
        RuleFor(x => x.Activity2).InclusiveBetween(0, 100);
        RuleFor(x => x.Activity3).InclusiveBetween(0, 100);
    }
}