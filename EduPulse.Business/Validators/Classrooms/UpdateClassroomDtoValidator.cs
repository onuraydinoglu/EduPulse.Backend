using EduPulse.DTOs.Classrooms;
using FluentValidation;

namespace EduPulse.Business.Validators.Classrooms;

public class UpdateClassroomDtoValidator : AbstractValidator<UpdateClassroomDto>
{
    public UpdateClassroomDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Sınıf Id boş olamaz.");

        RuleFor(x => x.SchoolId)
            .NotEmpty().WithMessage("Okul seçilmelidir.");

        RuleFor(x => x.Grade)
            .NotEmpty().WithMessage("Sınıf seviyesi boş olamaz.");

        RuleFor(x => x.Section)
            .NotEmpty().WithMessage("Şube boş olamaz.");
    }
}