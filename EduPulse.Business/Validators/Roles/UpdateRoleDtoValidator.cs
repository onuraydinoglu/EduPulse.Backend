using EduPulse.DTOs.Roles;
using FluentValidation;

namespace EduPulse.Business.Validators.Roles;

public class UpdateRoleDtoValidator : AbstractValidator<UpdateRoleDto>
{
    public UpdateRoleDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Rol Id boş olamaz.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Rol adı boş olamaz.")
            .MinimumLength(2).WithMessage("Rol adı en az 2 karakter olmalıdır.")
            .MaximumLength(50).WithMessage("Rol adı en fazla 50 karakter olmalıdır.");
    }
}