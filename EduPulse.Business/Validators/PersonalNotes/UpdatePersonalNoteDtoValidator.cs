using EduPulse.DTOs.PersonalNotes;
using FluentValidation;

namespace EduPulse.Business.Validators.PersonalNotes;

public class UpdatePersonalNoteDtoValidator : AbstractValidator<UpdatePersonalNoteDto>
{
    public UpdatePersonalNoteDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Not Id boş olamaz.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Not başlığı boş olamaz.")
            .MaximumLength(100).WithMessage("Not başlığı en fazla 100 karakter olabilir.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Not içeriği boş olamaz.")
            .MaximumLength(2000).WithMessage("Not içeriği en fazla 2000 karakter olabilir.");
    }
}