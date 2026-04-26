using EduPulse.DTOs.Classrooms;
using FluentValidation;

namespace EduPulse.Business.Validators.Classrooms;

public class CreateClassroomDtoValidator : AbstractValidator<CreateClassroomDto>
{
    public CreateClassroomDtoValidator()
    {
        RuleFor(x => x.SchoolId)
            .NotEmpty().WithMessage("Okul Id boş olamaz.");

        RuleFor(x => x.Grade)
            .InclusiveBetween(1, 12).WithMessage("Sınıf 1-12 arasında olmalı.");

        RuleFor(x => x.Section)
            .NotEmpty().WithMessage("Şube boş olamaz.")
            .MaximumLength(2);
    }
}