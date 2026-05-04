using EduPulse.DTOs.EventMembers;
using FluentValidation;

namespace EduPulse.Business.Validators.EventMembers;

public class UpdateEventMemberPaymentDtoValidator : AbstractValidator<UpdateEventMemberPaymentDto>
{
    public UpdateEventMemberPaymentDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Etkinlik üye Id boş olamaz.");

        RuleFor(x => x.PaidAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Ödenen tutar 0'dan küçük olamaz.");
    }
}