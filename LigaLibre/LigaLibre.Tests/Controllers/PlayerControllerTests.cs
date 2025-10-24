using FluentValidation;
using FluentValidation.Results;
using LigaLibre.API.Controllers;
using LigaLibre.Application.DTOs;
using LigaLibre.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LigaLibre.Tests.Controllers;

public class PlayerControllerTests
{
    private readonly Mock<IPlayerService> _mockService;
    private readonly Mock<IValidator<CreatePlayerDto>> _mockCreateValidator;
    private readonly Mock<IValidator<UpdatePlayerDto>> _mockUpdateValidator;
    private readonly PlayersController _controller;

    public PlayerControllerTests()
    {
        _mockService = new Mock<IPlayerService>();
        _mockCreateValidator = new Mock<IValidator<CreatePlayerDto>>();
        _mockUpdateValidator = new Mock<IValidator<UpdatePlayerDto>>();
        _controller = new PlayersController(_mockService.Object, _mockCreateValidator.Object, _mockUpdateValidator.Object);
    }

    /// <summary>
    /// Verifica que GetAllPlayers retorna Ok con lista de jugadores
    /// </summary>
    [Fact]
    public async Task GetAllPlayers_ReturnsOk()
    {
        //Arrange
        var players = new List<PlayerDto> { new PlayerDto { Id = 1, FirstName = "Juan" } };
        _mockService.Setup(s => s.GetAllPlayers()).ReturnsAsync(players);

        //Act
        var result = await _controller.GetAllPlayers();

        //Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(players, okResult.Value);
    }

    /// <summary>
    /// Verifica que GetPlayersByClub retorna Ok con jugadores del club
    /// </summary>
    [Fact]
    public async Task GetPlayersByClub_ReturnsOk()
    {
        //Arrange
        var players = new List<PlayerDto> { new PlayerDto { Id = 1, ClubId = 1 } };
        _mockService.Setup(s => s.GetPlayersByClubAsync(1)).ReturnsAsync(players);

        //Act
        var result = await _controller.GetPlayersByClub(1);

        //Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(players, okResult.Value);
    }

    /// <summary>
    /// Verifica que CreatePlayer con datos válidos retorna Created
    /// </summary>
    [Fact]
    public async Task CreatePlayer_ValidDto_ReturnsCreated()
    {
        //Arrange
        var createDto = new CreatePlayerDto { FirstName = "Juan" };
        var playerDto = new PlayerDto { Id = 1, FirstName = "Juan" };
        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, default)).ReturnsAsync(new ValidationResult());
        _mockService.Setup(s => s.CreatePlayerAsync(createDto)).ReturnsAsync(playerDto);

        //Act
        var result = await _controller.CreatePlayer(createDto);

        //Assert
        var createdResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(201, createdResult.StatusCode);
    }

    /// <summary>
    /// Verifica que CreatePlayer con datos inválidos retorna BadRequest
    /// </summary>
    [Fact]
    public async Task CreatePlayer_InvalidDto_ReturnsBadRequest()
    {
        //Arrange
        var createDto = new CreatePlayerDto { FirstName = "" };
        var validationResult = new ValidationResult(new[] { new ValidationFailure("FirstName", "Required") });
        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, default)).ReturnsAsync(validationResult);

        //Act
        var result = await _controller.CreatePlayer(createDto);

        //Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    /// <summary>
    /// Verifica que UpdatePlayer con datos válidos retorna Ok
    /// </summary>
    [Fact]
    public async Task UpdatePlayer_ValidDto_ReturnsOk()
    {
        //Arrange
        var updateDto = new UpdatePlayerDto { Id = 1, FirstName = "Juan" };
        var playerDto = new PlayerDto { Id = 1, FirstName = "Juan" };
        _mockUpdateValidator.Setup(v => v.ValidateAsync(updateDto, default)).ReturnsAsync(new ValidationResult());
        _mockService.Setup(s => s.UpdatePlayerAsync(updateDto)).ReturnsAsync(playerDto);

        //Act
        var result = await _controller.UpdatePlayer(updateDto);

        //Assert
        Assert.IsType<OkObjectResult>(result);
    }

    /// <summary>
    /// Verifica que DeletePlayer retorna Ok
    /// </summary>
    [Fact]
    public async Task DeletePlayer_ReturnsOk()
    {
        //Arrange
        _mockService.Setup(s => s.DeletePlayerAsync(1)).ReturnsAsync(true);

        //Act
        var result = await _controller.DeletePlayers(1);

        //Assert
        Assert.IsType<OkObjectResult>(result);
    }
}
