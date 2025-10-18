using LigaLibre.Application.DTOs;
using LigaLibre.Application.Validators;

namespace LigaLibre.Tests.Validators;

public class UpdateClubValidatorTests
{
    private readonly UpdateClubValidator _validator;

    public UpdateClubValidatorTests()
    {
        _validator = new UpdateClubValidator();
    }

    /// <summary>
    /// Verifica que un DTO válido pasa la validación
    /// </summary>
    [Fact]
    public void Validate_ValidDto_PassesValidation()
    {
        //Arrange
        var dto = new UpdateClubDto
        {
            Name = "Boca Juniors",
            City = "Buenos Aires",
            Email = "boca@example.com",
            Phone = "1234567890",
            Address = "Brandsen 805",
            StadiumName = "La Bombonera",
            NumberOfPartners = 300000
        };

        //Act
        var result = _validator.Validate(dto);

        //Assert
        Assert.True(result.IsValid);
    }

    /// <summary>
    /// Verifica que Name vacío falla la validación
    /// </summary>
    [Fact]
    public void Validate_EmptyName_FailsValidation()
    {
        //Arrange
        var dto = new UpdateClubDto { Name = "" };

        //Act
        var result = _validator.Validate(dto);

        //Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    /// <summary>
    /// Verifica que Email inválido falla la validación
    /// </summary>
    [Fact]
    public void Validate_InvalidEmail_FailsValidation()
    {
        //Arrange
        var dto = new UpdateClubDto { Name = "Boca", Email = "invalid-email" };

        //Act
        var result = _validator.Validate(dto);

        //Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
    }
}
