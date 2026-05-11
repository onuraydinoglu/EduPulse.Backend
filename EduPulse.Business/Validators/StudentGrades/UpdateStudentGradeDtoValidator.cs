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

        RuleFor(x => x)
            .Must(HasAtLeastOneGrade)
            .WithMessage("En az bir not alanı girilmelidir.");

        RuleFor(x => x.Exam1)
            .InclusiveBetween(0, 100)
            .When(x => x.Exam1.HasValue)
            .WithMessage("1. sınav notu 0 ile 100 arasında olmalıdır.");

        RuleFor(x => x.Exam2)
            .InclusiveBetween(0, 100)
            .When(x => x.Exam2.HasValue)
            .WithMessage("2. sınav notu 0 ile 100 arasında olmalıdır.");

        RuleFor(x => x.Project)
            .InclusiveBetween(0, 100)
            .When(x => x.Project.HasValue)
            .WithMessage("Proje notu 0 ile 100 arasında olmalıdır.");

        RuleFor(x => x.Activity1)
            .InclusiveBetween(0, 100)
            .When(x => x.Activity1.HasValue)
            .WithMessage("Sınıf içi 1 notu 0 ile 100 arasında olmalıdır.");

        RuleFor(x => x.Activity2)
            .InclusiveBetween(0, 100)
            .When(x => x.Activity2.HasValue)
            .WithMessage("Sınıf içi 2 notu 0 ile 100 arasında olmalıdır.");

        RuleFor(x => x.Activity3)
            .InclusiveBetween(0, 100)
            .When(x => x.Activity3.HasValue)
            .WithMessage("Sınıf içi 3 notu 0 ile 100 arasında olmalıdır.");
    }

    private static bool HasAtLeastOneGrade(UpdateStudentGradeDto dto)
    {
        return dto.Exam1.HasValue ||
               dto.Exam2.HasValue ||
               dto.Project.HasValue ||
               dto.Activity1.HasValue ||
               dto.Activity2.HasValue ||
               dto.Activity3.HasValue;
    }
}