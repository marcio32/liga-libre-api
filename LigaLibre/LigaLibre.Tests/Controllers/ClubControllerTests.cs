using FluentValidation;
using LigaLibre.API.Controllers;
using LigaLibre.Application.DTOs;
using LigaLibre.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentValidation.Results;
namespace LigaLibre.Tests.Controllers;


public class ClubControllerTests
{
    private readonly Mock<IClubService> _mockService;
    private readonly Mock<IValidator<CreateClubDto>> _mockValidator;
    private readonly Mock<IValidator<UpdateClubDto>> _mockUpdateValidator;
    private readonly ClubController _controller;
    public ClubControllerTests()
    {
        _mockService = new Mock<IClubService>();
        _mockValidator = new Mock<IValidator<CreateClubDto>>();
        _mockUpdateValidator = new Mock<IValidator<UpdateClubDto>>();
        _controller = new ClubController(_mockService.Object, _mockValidator.Object, _mockUpdateValidator.Object);
    }


    [Fact]
    public async Task GetById_ExistingId_ReturnsOk()
    {
        //Arrange
        var club = new ClubDto { Id = 1, Name = "Boca" };
        _mockService.Setup(s => s.GetClubByIdAsync(1)).ReturnsAsync(club);

        //Act
        var result = await _controller.GetById(1);

        //Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<ClubDto>(okResult.Value);
        Assert.Equal(club.Id, returnValue.Id);
    }

    [Fact]
    public async Task GetById_NonExistingId_ReturnsNotFound()
    {
        //Arrage
        _mockService.Setup(s => s.GetClubByIdAsync(999)).ReturnsAsync((ClubDto?)null);

        //Act
        var result = await _controller.GetById(999);

        //Assert
        Assert.IsType<NotFoundResult>(result);

    }

    [Fact]
    public async Task Create_ValidDto_ReturnsCreated()
    {
        //Arrange
        var createDto = new CreateClubDto { Name = "River" };
        var clubDto = new ClubDto { Id = 1, Name = "River" };
        _mockValidator.Setup(v => v.ValidateAsync(createDto, default)).ReturnsAsync(new ValidationResult());
        _mockService.Setup(s => s.CreateClubAsync(createDto)).ReturnsAsync(clubDto);

        //Act
        var result = await _controller.CreateClub(createDto);

        //Assert
        var okCreatedResult = Assert.IsType<ObjectResult>(result);
        var CreatedResult = Assert.IsType<ClubDto>(okCreatedResult.Value);
        Assert.Equal(201, okCreatedResult.StatusCode);
        Assert.Equal(clubDto.Id, CreatedResult.Id);
    }

    [Fact]
    public async Task Create_InvalidDto_ReturnsBadRequest()
    {
        //Arrange
        var createDto = new CreateClubDto { Name = "" };
        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("Name", "Name is required")
        });

        _mockValidator.Setup(v => v.ValidateAsync(createDto, default)).ReturnsAsync(validationResult);

        //Act
        var result = await _controller.CreateClub(createDto);

        //Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errors = Assert.IsType<List<ValidationFailure>>(badRequestResult.Value);
        Assert.Single(errors);
        Assert.Equal("Name", errors[0].PropertyName);
        Assert.Equal("Name is required", errors[0].ErrorMessage);
    }

    /// <summary>
    /// Verifica que GetAll retorna Ok con lista de clubes
    /// </summary>
    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        //Arrange
        var clubs = new List<ClubDto> { new ClubDto { Id = 1, Name = "Boca" } };
        _mockService.Setup(s => s.GetAllClubsAsync()).ReturnsAsync(clubs);

        //Act
        var result = await _controller.GetAll();

        //Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(clubs, okResult.Value);
    }

    /// <summary>
    /// Verifica que UpdateClub con datos válidos retorna Ok
    /// </summary>
    [Fact]
    public async Task UpdateClub_ValidDto_ReturnsOk()
    {
        //Arrange
        var updateDto = new UpdateClubDto { Id = 1, Name = "River" };
        var clubDto = new ClubDto { Id = 1, Name = "River" };
        _mockUpdateValidator.Setup(v => v.ValidateAsync(updateDto, default)).ReturnsAsync(new ValidationResult());
        _mockService.Setup(s => s.UpdateClubAsync(updateDto)).ReturnsAsync(clubDto);

        //Act
        var result = await _controller.UpdateClub(updateDto);

        //Assert
        Assert.IsType<OkObjectResult>(result);
    }

    /// <summary>
    /// Verifica que Delete retorna Ok
    /// </summary>
    [Fact]
    public async Task Delete_ReturnsOk()
    {
        //Arrange
        _mockService.Setup(s => s.DeleteClubAsync(1)).ReturnsAsync(true);

        //Act
        var result = await _controller.Delete(1);

        //Assert
        Assert.IsType<OkObjectResult>(result);
    }
}
        

