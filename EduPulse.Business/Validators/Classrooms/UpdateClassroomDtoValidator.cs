using EduPulse.DTOs.Classrooms;
using FluentValidation;

namespace EduPulse.Business.Validators.Classrooms;

public class UpdateClassroomDtoValidator : AbstractValidator<UpdateClassroomDto>
{
    public UpdateClassroomDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Sınıf Id boş olamaz.");

        RuleFor(x => x.Grade)
            .GreaterThan(0).WithMessage("Sınıf seviyesi geçerli olmalıdır.");

        RuleFor(x => x.Section)
            .NotEmpty().WithMessage("Şube boş olamaz.")
            .MaximumLength(10).WithMessage("Şube en fazla 10 karakter olabilir.");
    }
}