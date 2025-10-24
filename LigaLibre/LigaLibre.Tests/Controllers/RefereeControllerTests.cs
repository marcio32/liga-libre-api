using FluentValidation;
using FluentValidation.Results;
using LigaLibre.API.Controllers;
using LigaLibre.Application.DTOs;
using LigaLibre.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LigaLibre.Tests.Controllers;

public class RefereeControllerTests
{
    private readonly Mock<IRefereeService> _mockService;
    private readonly Mock<IValidator<CreateRefereeDto>> _mockCreateValidator;
    private readonly Mock<IValidator<UpdateRefereeDto>> _mockUpdateValidator;
    private readonly RefereeController _controller;

    public RefereeControllerTests()
    {
        _mockService = new Mock<IRefereeService>();
        _mockCreateValidator = new Mock<IValidator<CreateRefereeDto>>();
        _mockUpdateValidator = new Mock<IValidator<UpdateRefereeDto>>();
        _controller = new RefereeController(_mockService.Object, _mockCreateValidator.Object, _mockUpdateValidator.Object);
    }

    /// <summary>
    /// Verifica que GetAllReferees retorna Ok con lista de árbitros
    /// </summary>
    [Fact]
    public async Task GetAllReferees_ReturnsOk()
    {
        //Arrange
        var referees = new List<RefereeDto> { new RefereeDto { Id = 1, FirstName = "Carlos" } };
        _mockService.Setup(s => s.GetAllRefereesAsync()).ReturnsAsync(referees);

        //Act
        var result = await _controller.GetAllReferees();

        //Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(referees, okResult.Value);
    }

    /// <summary>
    /// Verifica que GetActiveReferees retorna Ok con árbitros activos
    /// </summary>
    [Fact]
    public async Task GetActiveReferees_ReturnsOk()
    {
        //Arrange
        var referees = new List<RefereeDto> { new RefereeDto { Id = 1, IsActive = true } };
        _mockService.Setup(s => s.GetActiveRefereesAsync()).ReturnsAsync(referees);

        //Act
        var result = await _controller.GetActiveReferees();

        //Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(referees, okResult.Value);
    }

    /// <summary>
    /// Verifica que CreateReferee con datos válidos retorna Created
    /// </summary>
    [Fact]
    public async Task CreateReferee_ValidDto_ReturnsCreated()
    {
        //Arrange
        var createDto = new CreateRefereeDto { FirstName = "Carlos" };
        var refereeDto = new RefereeDto { Id = 1, FirstName = "Carlos" };
        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, default)).ReturnsAsync(new ValidationResult());
        _mockService.Setup(s => s.CreateRefereeAsync(createDto)).ReturnsAsync(refereeDto);

        //Act
        var result = await _controller.CreateReferee(createDto);

        //Assert
        var createdResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(201, createdResult.StatusCode);
    }

    /// <summary>
    /// Verifica que CreateReferee con datos inválidos retorna BadRequest
    /// </summary>
    [Fact]
    public async Task CreateReferee_InvalidDto_ReturnsBadRequest()
    {
        //Arrange
        var createDto = new CreateRefereeDto { FirstName = "" };
        var validationResult = new ValidationResult(new[] { new ValidationFailure("FirstName", "Required") });
        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, default)).ReturnsAsync(validationResult);

        //Act
        var result = await _controller.CreateReferee(createDto);

        //Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    /// <summary>
    /// Verifica que UpdateReferee con datos válidos retorna Ok
    /// </summary>
    [Fact]
    public async Task UpdateReferee_ValidDto_ReturnsOk()
    {
        //Arrange
        var updateDto = new UpdateRefereeDto { Id = 1, FirstName = "Carlos" };
        var refereeDto = new RefereeDto { Id = 1, FirstName = "Carlos" };
        _mockUpdateValidator.Setup(v => v.ValidateAsync(updateDto, default)).ReturnsAsync(new ValidationResult());
        _mockService.Setup(s => s.UpdateRefereeAsync(updateDto)).ReturnsAsync(refereeDto);

        //Act
        var result = await _controller.UpdateReferee(updateDto);

        //Assert
        Assert.IsType<OkObjectResult>(result);
    }

    /// <summary>
    /// Verifica que DeleteReferee con ID existente retorna NoContent
    /// </summary>
    [Fact]
    public async Task DeleteReferee_ExistingId_ReturnsNoContent()
    {
        //Arrange
        _mockService.Setup(s => s.DeleteRefereeAsync(1)).ReturnsAsync(true);

        //Act
        var result = await _controller.DeleteReferee(1);

        //Assert
        Assert.IsType<NoContentResult>(result);
    }

    /// <summary>
    /// Verifica que DeleteReferee con ID inexistente retorna NotFound
    /// </summary>
    [Fact]
    public async Task DeleteReferee_NonExistingId_ReturnsNotFound()
    {
        //Arrange
        _mockService.Setup(s => s.DeleteRefereeAsync(999)).ReturnsAsync(false);

        //Act
        var result = await _controller.DeleteReferee(999);

        //Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains("999", notFoundResult.Value?.ToString());
    }
}
