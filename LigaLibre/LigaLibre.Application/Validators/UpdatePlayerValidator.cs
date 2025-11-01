using FluentValidation;
using LigaLibre.Application.DTOs;

namespace LigaLibre.Application.Validators
{
    public class UpdatePlayerValidator : AbstractValidator<UpdatePlayerDto>
    {
        public UpdatePlayerValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithName("El nombre es requerido")
                .MaximumLength(50).WithMessage("El nombre no puede exceder 50 catacteres");

            RuleFor(x => x.LastName)
                .NotEmpty().WithName("El apellido es requerido")
                .MaximumLength(50).WithMessage("El apellido no puede exceder 50 catacteres");

            RuleFor(x => x.Age)
                .GreaterThan(16).WithMessage("La edad minima es 17 años")
                .LessThan(50).WithMessage("La edad maxima es 49 años");

            RuleFor(x => x.Position)
                .NotEmpty().WithMessage("La posicion es requerida")
                .Must(BeValidPosition).WithMessage("Posicion no valida");

            RuleFor(x => x.JerseyNumber)
                .GreaterThan(0).WithMessage("El numero de camiseta debe ser mayor a 0")
                .LessThanOrEqualTo(99).WithMessage("El numero de camiseta no puede ser mayor a 99");

            RuleFor(x => x.Weight)
                .GreaterThan(50).WithMessage("El peso minimo es de 50kg")
                .LessThan(120).WithMessage("El peso maximo es 120kg");
            
            RuleFor(x => x.Height)
                .GreaterThan(1.50m).WithMessage("La altura minima es 1.40m")
                .LessThan(2.20m).WithMessage("La altura maxima es 2.20m");

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateTime.Now.AddYears(-16)).WithMessage("Debe ser mayor de 16 años");

        }

        private static bool BeValidPosition(string position)
        {
            var validPositions = new[] { "Portero", "Defensa", "Mediocampista", "Delantero" };
            return validPositions.Contains(position);
        }
    }
}
