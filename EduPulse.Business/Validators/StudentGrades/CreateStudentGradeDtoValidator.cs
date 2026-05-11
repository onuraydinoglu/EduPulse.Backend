using EduPulse.DTOs.StudentGrades;
using FluentValidation;

namespace EduPulse.Business.Validators.StudentGrades;

public class CreateStudentGradeDtoValidator : AbstractValidator<CreateStudentGradeDto>
{
    public CreateStudentGradeDtoValidator()
    {
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

        RuleFor(x => x)
            .Must(HasAtLeastOneGrade)
            .WithMessage("En az bir not alanı girilmelidir.");

        AddGradeRule(x => x.Exam1, "1. sınav");
        AddGradeRule(x => x.Exam2, "2. sınav");
        AddGradeRule(x => x.Project, "Proje");
        AddGradeRule(x => x.Activity1, "Sınıf içi 1");
        AddGradeRule(x => x.Activity2, "Sınıf içi 2");
        AddGradeRule(x => x.Activity3, "Sınıf içi 3");
    }

    private void AddGradeRule(
        System.Linq.Expressions.Expression<Func<CreateStudentGradeDto, double?>> expression,
        string fieldName)
    {
        RuleFor(expression)
            .InclusiveBetween(0, 100)
            .When(x => expression.Compile()(x).HasValue)
            .WithMessage($"{fieldName} notu 0 ile 100 arasında olmalıdır.");
    }

    private static bool HasAtLeastOneGrade(CreateStudentGradeDto dto)
    {
        return dto.Exam1.HasValue ||
               dto.Exam2.HasValue ||
               dto.Project.HasValue ||
               dto.Activity1.HasValue ||
               dto.Activity2.HasValue ||
               dto.Activity3.HasValue;
    }
}