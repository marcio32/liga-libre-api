using LigaLibre.Application.DTOs;
using LigaLibre.Application.Validators;

namespace LigaLibre.Tests.Validators;

public class UpdatePlayerValidatorTests
{
    private readonly UpdatePlayerValidator _validator;

    public UpdatePlayerValidatorTests()
    {
        _validator = new UpdatePlayerValidator();
    }

    /// <summary>
    /// Verifica que un DTO válido pasa la validación
    /// </summary>
    [Fact]
    public void Validate_ValidDto_PassesValidation()
    {
        //Arrange
        var dto = new UpdatePlayerDto
        {
            FirstName = "Lionel",
            LastName = "Messi",
            DateOfBirth = new DateTime(1987, 6, 24),
            Position = "Delantero",
            JerseyNumber = 10,
            Height = 1.70m,
            Weight = 72,
            Age = 37,
            ClubId = 1
        };

        //Act
        var result = _validator.Validate(dto);

        //Assert
        Assert.True(result.IsValid);
    }

    /// <summary>
    /// Verifica que FirstName vacío falla la validación
    /// </summary>
    [Fact]
    public void Validate_EmptyFirstName_FailsValidation()
    {
        //Arrange
        var dto = new UpdatePlayerDto { FirstName = "" };

        //Act
        var result = _validator.Validate(dto);

        //Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "FirstName");
    }

    /// <summary>
    /// Verifica que Position inválida falla la validación
    /// </summary>
    [Fact]
    public void Validate_InvalidPosition_FailsValidation()
    {
        //Arrange
        var dto = new UpdatePlayerDto
        {
            FirstName = "Juan",
            LastName = "Perez",
            Position = "InvalidPosition"
        };

        //Act
        var result = _validator.Validate(dto);

        //Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Position");
    }
}
