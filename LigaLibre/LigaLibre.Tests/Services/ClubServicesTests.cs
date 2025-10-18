using LigaLibre.Application.DTOs;
using LigaLibre.Domain.Entities;
using LigaLibre.Domain.Interfaces;
using Moq;

namespace LigaLibre.Tests.Services;

public class ClubServicesTests
{
    private readonly Mock<IClubRepository> _mockRepository;
    public ClubServicesTests()
    {
        _mockRepository = new Mock<IClubRepository>();
    }

    [Fact]
    public async Task GetClubByIdAsync_ExistingId_ReturnsClub()
    {
        // Arrange
        var club = new Club { Id = 1, Name = "Boca", City = "Buenos Aires" };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(club);

        // Act
        var result = await _mockRepository.Object.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(club.Id, result.Id);
        Assert.Equal(club.Name, result.Name);
        Assert.Equal(club.City, result.City);
    }

    [Fact]
    public async Task CreateClubAsync_ValidDto_ReturnsCreatedClub()
    {
        // Arrange
        var createDto = new CreateClubDto
        {
            Name = "River",
            City = "Buenos Aires",
            Email = "info@river.com",
            StadiumName = "Monumental",
            NumberOfPartners = 50000
        };
        var club = new Club { Id = 1, Name = "River", City = "Buenos Aires" };
        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Club>())).ReturnsAsync(club);

        //Act
        var result = await _mockRepository.Object.CreateAsync(new Club
        {
            Name = createDto.Name,
            City = createDto.City,
            Email = createDto.Email,
            StadiumName = createDto.StadiumName,
            NumberOfPartners = createDto.NumberOfPartners
        });

        // Assert
        Assert.NotNull(result);
        Assert.Equal(club.Id, result.Id);
        Assert.Equal(club.Name, result.Name);
        Assert.Equal(club.City, result.City);
        Assert.Equal(club.Email, result.Email);
        Assert.Equal(club.StadiumName, result.StadiumName);

    }
}
