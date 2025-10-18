using FluentValidation;
using LigaLibre.Application.DTOs;

namespace LigaLibre.Application.Validators;

public class CreateRefereeValidator : AbstractValidator<CreateRefereeDto>
{
    public CreateRefereeValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithName("El nombre es requerido")
            .MaximumLength(50).WithMessage("El nombre no puede exceder 50 catacteres");

        RuleFor(x => x.LastName)
            .NotEmpty().WithName("El apellido es requerido")
            .MaximumLength(50).WithMessage("El apellido no puede exceder 50 catacteres");

        RuleFor(x => x.LicenseNumber)
            .NotEmpty().WithName("El número de licencia es requerido")
            .MaximumLength(50).WithMessage("El número de licencia no puede exceder 50 catacteres");

        RuleFor(x=> x.Category)
            .IsInEnum().WithMessage("La categoría no es válida");

    }
}

