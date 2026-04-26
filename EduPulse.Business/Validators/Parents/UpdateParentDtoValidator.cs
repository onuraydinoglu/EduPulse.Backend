using EduPulse.DTOs.Parents;
using FluentValidation;

namespace EduPulse.Business.Validators.Parents;

public class UpdateParentDtoValidator : AbstractValidator<UpdateParentDto>
{
    public UpdateParentDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Veli Id boş olamaz.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Veli adı boş olamaz.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Veli soyadı boş olamaz.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Telefon numarası boş olamaz.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email boş olamaz.")
            .EmailAddress().WithMessage("Geçerli bir email giriniz.");

        RuleFor(x => x.SchoolId)
            .NotEmpty().WithMessage("Okul seçilmelidir.");
    }
}