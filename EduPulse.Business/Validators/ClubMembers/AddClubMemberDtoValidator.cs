using EduPulse.DTOs.ClubMembers;
using FluentValidation;

namespace EduPulse.Business.Validators.ClubMembers;

public class CreateClubMemberDtoValidator : AbstractValidator<CreateClubMemberDto>
{
    public CreateClubMemberDtoValidator()
    {
        RuleFor(x => x.ClubId)
            .NotEmpty()
            .WithMessage("Kulüp seçiniz.");

        RuleFor(x => x.StudentId)
            .NotEmpty()
            .WithMessage("Öğrenci seçiniz.");
    }
}