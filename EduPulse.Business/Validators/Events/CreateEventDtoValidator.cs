using EduPulse.DTOs.Events;
using FluentValidation;

namespace EduPulse.Business.Validators.Events;

public class CreateEventDtoValidator : AbstractValidator<CreateEventDto>
{
    public CreateEventDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Etkinlik adı boş olamaz.");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Etkinlik yeri boş olamaz.");

        RuleFor(x => x.EventDate)
            .NotEmpty().WithMessage("Etkinlik tarihi boş olamaz.");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Başlangıç saati boş olamaz.");

        RuleFor(x => x.PricePerStudent)
            .GreaterThan(0).When(x => x.IsPaid)
            .WithMessage("Ücretli etkinliklerde kişi başı ücret 0'dan büyük olmalıdır.");

        RuleFor(x => x.PricePerStudent)
            .Equal(0).When(x => !x.IsPaid)
            .WithMessage("Ücretsiz etkinliklerde ücret 0 olmalıdır.");

        RuleFor(x => x.Quota)
            .GreaterThan(0).When(x => x.Quota.HasValue)
            .WithMessage("Kontenjan 0'dan büyük olmalıdır.");
    }
}