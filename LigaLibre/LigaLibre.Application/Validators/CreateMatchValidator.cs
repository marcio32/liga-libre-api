using FluentValidation;
using LigaLibre.Application.DTOs;

namespace LigaLibre.Application.Validators;

public class CreateMatchValidator : AbstractValidator<CreateMatchDto>
{
    public CreateMatchValidator()
    {
        RuleFor(x=> x.Round)
            .GreaterThan(0).WithMessage("La jornada debe ser mayor a 0");
        RuleFor(x=> x.HomeClubId)
            .GreaterThan(0).WithMessage("El ID del club local es requerido");
        RuleFor(x=> x.AwayClubId)
            .GreaterThan(0).WithMessage("El ID del club visitante es requerido")
            .NotEqual(x=> x.HomeClubId).WithMessage("El club visitante no puede ser el mismo que el club local");
        RuleFor(x=>x.Stadium)
            .GreaterThan(0).WithMessage("El estadio es requerido");
        RuleFor(x=>x.MatchDate)
            .NotEmpty().WithMessage("La fecha del partido es requerida")
            .GreaterThan(DateTime.Now).WithMessage("La fecha del partido debe ser futura");

    }
}

