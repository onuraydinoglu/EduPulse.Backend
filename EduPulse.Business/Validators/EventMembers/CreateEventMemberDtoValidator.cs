using EduPulse.DTOs.EventMembers;
using FluentValidation;

namespace EduPulse.Business.Validators.EventMembers;

public class CreateEventMemberDtoValidator : AbstractValidator<CreateEventMemberDto>
{
    public CreateEventMemberDtoValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty().WithMessage("Etkinlik seçilmelidir.");

        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Öğrenci seçilmelidir.");

        RuleFor(x => x.PaidAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Ödenen tutar 0'dan küçük olamaz.");
    }
}