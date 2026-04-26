using EduPulse.DTOs.Parents;
using FluentValidation;

namespace EduPulse.Business.Validators.Parents;

public class CreateParentDtoValidator : AbstractValidator<CreateParentDto>
{
    public CreateParentDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Veli adı boş olamaz.")
            .MaximumLength(50);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Veli soyadı boş olamaz.")
            .MaximumLength(50);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Telefon numarası boş olamaz.")
            .Matches(@"^05\d{9}$").WithMessage("Telefon 05 ile başlamalı.");

        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.SchoolId)
            .NotEmpty().WithMessage("Okul Id boş olamaz.");
    }
}