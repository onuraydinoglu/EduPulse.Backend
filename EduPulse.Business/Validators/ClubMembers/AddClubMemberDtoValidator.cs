using EduPulse.DTOs.ClubMembers;
using FluentValidation;

namespace EduPulse.Business.Validators.ClubMembers;

public class AddClubMemberDtoValidator : AbstractValidator<AddClubMemberDto>
{
    public AddClubMemberDtoValidator()
    {
        RuleFor(x => x.ClubId)
            .NotEmpty().WithMessage("Kulüp seçilmelidir.");

        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Öğrenci seçilmelidir.");
    }
}