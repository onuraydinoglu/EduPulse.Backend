using EduPulse.DTOs.Clubs;
using FluentValidation;

namespace EduPulse.Business.Validators.Clubs;

public class UpdateClubDtoValidator : AbstractValidator<UpdateClubDto>
{
    public UpdateClubDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Kulüp Id boş olamaz.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Kulüp adı boş olamaz.")
            .MaximumLength(100).WithMessage("Kulüp adı en fazla 100 karakter olabilir.");

        RuleFor(x => x.AdvisorTeacherId)
            .NotEmpty().WithMessage("Kulüp hocası seçilmelidir.");
    }
}