using FluentValidation;
using FluentValidation.Results;
using LigaLibre.API.Controllers;
using LigaLibre.Application.DTOs;
using LigaLibre.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LigaLibre.Tests.Controllers;

public class MatchControllerTests
{
    private readonly Mock<IMatchService> _mockService;
    private readonly Mock<IValidator<CreateMatchDto>> _mockCreateValidator;
    private readonly Mock<IValidator<UpdateMatchDto>> _mockUpdateValidator;
    private readonly MatchController _controller;

    public MatchControllerTests()
    {
        _mockService = new Mock<IMatchService>();
        _mockCreateValidator = new Mock<IValidator<CreateMatchDto>>();
        _mockUpdateValidator = new Mock<IValidator<UpdateMatchDto>>();
        _controller = new MatchController(_mockService.Object, _mockCreateValidator.Object, _mockUpdateValidator.Object);
    }

    /// <summary>
    /// Verifica que GetAllMatches retorna Ok con lista de partidos
    /// </summary>
    [Fact]
    public async Task GetAllMatches_ReturnsOk()
    {
        //Arrange
        var matches = new List<MatchDto> { new MatchDto { Id = 1 } };
        _mockService.Setup(s => s.GetAllMatchesAsync()).ReturnsAsync(matches);

        //Act
        var result = await _controller.GetAllMatches();

        //Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(matches, okResult.Value);
    }

    /// <summary>
    /// Verifica que GetMatchById con ID existente retorna Ok
    /// </summary>
    [Fact]
    public async Task GetMatchById_ExistingId_ReturnsOk()
    {
        //Arrange
        var match = new MatchDto { Id = 1 };
        _mockService.Setup(s => s.GetMatchByIdAsync(1)).ReturnsAsync(match);

        //Act
        var result = await _controller.GetMatchById(1);

        //Assert
        Assert.IsType<OkObjectResult>(result.Result);
    }

    /// <summary>
    /// Verifica que GetMatchById con ID inexistente retorna NotFound
    /// </summary>
    [Fact]
    public async Task GetMatchById_NonExistingId_ReturnsNotFound()
    {
        //Arrange
        _mockService.Setup(s => s.GetMatchByIdAsync(999)).ReturnsAsync((MatchDto?)null);

        //Act
        var result = await _controller.GetMatchById(999);

        //Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    /// <summary>
    /// Verifica que CreateMatch con datos válidos retorna Created
    /// </summary>
    [Fact]
    public async Task CreateMatch_ValidDto_ReturnsCreated()
    {
        //Arrange
        var createDto = new CreateMatchDto { HomeClubId = 1, AwayClubId = 2 };
        var matchDto = new MatchDto { Id = 1 };
        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, default)).ReturnsAsync(new ValidationResult());
        _mockService.Setup(s => s.CreateMatchAsync(createDto)).ReturnsAsync(matchDto);

        //Act
        var result = await _controller.CreateMatch(createDto);

        //Assert
        var createdResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(201, createdResult.StatusCode);
    }

    /// <summary>
    /// Verifica que UpdateMatch con datos válidos retorna Ok
    /// </summary>
    [Fact]
    public async Task UpdateMatch_ValidDto_ReturnsOk()
    {
        //Arrange
        var updateDto = new UpdateMatchDto { Id = 1, HomeClubId = 1 };
        var matchDto = new MatchDto { Id = 1 };
        _mockUpdateValidator.Setup(v => v.ValidateAsync(updateDto, default)).ReturnsAsync(new ValidationResult());
        _mockService.Setup(s => s.UpdateMatchAsync(updateDto)).ReturnsAsync(matchDto);

        //Act
        var result = await _controller.UpdateMatch(updateDto);

        //Assert
        Assert.IsType<OkObjectResult>(result.Result);
    }

    /// <summary>
    /// Verifica que DeleteMatch con ID existente retorna NoContent
    /// </summary>
    [Fact]
    public async Task DeleteMatch_ExistingId_ReturnsNoContent()
    {
        //Arrange
        _mockService.Setup(s => s.DeleteMatchAsync(1)).ReturnsAsync(true);

        //Act
        var result = await _controller.DeleteMatch(1);

        //Assert
        Assert.IsType<NoContentResult>(result);
    }

    /// <summary>
    /// Verifica que DeleteMatch con ID inexistente retorna NotFound
    /// </summary>
    [Fact]
    public async Task DeleteMatch_NonExistingId_ReturnsNotFound()
    {
        //Arrange
        _mockService.Setup(s => s.DeleteMatchAsync(999)).ReturnsAsync(false);

        //Act
        var result = await _controller.DeleteMatch(999);

        //Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
