using FluentValidation;
using LigaLibre.Application.DTOs;

namespace LigaLibre.Application.Validators;

public class UpdateClubValidator : AbstractValidator<UpdateClubDto>
{
    public UpdateClubValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del club es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("La ciudad es requerida")
            .MaximumLength(50).WithMessage("El nombre no puede exceder 100 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es requerido")
            .EmailAddress().WithMessage("Formato de email invalido")
            .MaximumLength(100).WithMessage("El email no puede exceder 100 caracteres");

        RuleFor(x => x.NumberOfPartners)
            .GreaterThan(0).WithMessage("El numero de socios debe ser mayor a 0");

        RuleFor(x => x.Phone)
            .MaximumLength(20).WithMessage("El telefono no puede exceder 20 caracteres");

        RuleFor(x => x.StadiumName)
            .NotEmpty().WithMessage("El nombre del estadio es requerido")
            .MaximumLength(100).WithMessage("El nombre del estadio no puede exceder 100 caracteres");
    }
}

