using LigaLibre.Application.DTOs;
using LigaLibre.Application.Interfaces;
using LigaLibre.Application.Services;
using LigaLibre.Domain.Entities;
using LigaLibre.Domain.Enums;
using LigaLibre.Domain.Interfaces;
using Moq;

namespace LigaLibre.Tests.Services;

public class RefereeServiceTests
{
    private readonly Mock<IRefereeRepository> _mockRepository;
    private readonly Mock<ISqsService> _mockSqs;
    private readonly Mock<IRedisCacheService> _mockCache;
    private readonly RefereeService _service;

    public RefereeServiceTests()
    {
        _mockRepository = new Mock<IRefereeRepository>();
        _mockSqs = new Mock<ISqsService>();
        _mockCache = new Mock<IRedisCacheService>();
        _service = new RefereeService(_mockRepository.Object, _mockSqs.Object, _mockCache.Object);
    }

    /// <summary>
    /// Verifica que GetAllRefereesAsync retorna árbitros desde caché
    /// </summary>
    [Fact]
    public async Task GetAllRefereesAsync_ReturnsCachedReferees()
    {
        //Arrange
        var referees = new List<RefereeDto> { new RefereeDto { Id = 1, FirstName = "Carlos" } };
        _mockCache.Setup(c => c.GetAsync<IEnumerable<RefereeDto>>("referees:all")).ReturnsAsync(referees);

        //Act
        var result = await _service.GetAllRefereesAsync();

        //Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    /// <summary>
    /// Verifica que GetActiveRefereesAsync retorna árbitros activos desde caché
    /// </summary>
    [Fact]
    public async Task GetActiveRefereesAsync_ReturnsCachedActiveReferees()
    {
        //Arrange
        var referees = new List<RefereeDto> { new RefereeDto { Id = 1, IsActive = true } };
        _mockCache.Setup(c => c.GetAsync<IEnumerable<RefereeDto>>("referees:active")).ReturnsAsync(referees);

        //Act
        var result = await _service.GetActiveRefereesAsync();

        //Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    /// <summary>
    /// Verifica que GetRefereeByIdAsync retorna árbitro desde caché
    /// </summary>
    [Fact]
    public async Task GetRefereeByIdAsync_ReturnsCachedReferee()
    {
        //Arrange
        var referee = new RefereeDto { Id = 1, FirstName = "Carlos" };
        _mockCache.Setup(c => c.GetAsync<RefereeDto?>("referees:1")).ReturnsAsync(referee);

        //Act
        var result = await _service.GetRefereeByIdAsync(1);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    /// <summary>
    /// Verifica que CreateRefereeAsync crea árbitro cuando no existe licencia duplicada
    /// </summary>
    [Fact]
    public async Task CreateRefereeAsync_CreatesRefereeWhenNoExistingLicense()
    {
        //Arrange
        var createDto = new CreateRefereeDto
        {
            FirstName = "Carlos",
            LastName = "Gomez",
            LicenseNumber = "LIC123",
        };
        _mockRepository.Setup(r => r.GetByLicenseNumberAsync("LIC123")).ReturnsAsync((Referee?)null);
        var referee = new Referee { Id = 1, FirstName = "Carlos", LicenseNumber = "LIC123" };
        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Referee>())).ReturnsAsync(referee);

        //Act
        var result = await _service.CreateRefereeAsync(createDto);

        //Assert
        Assert.NotNull(result);
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Referee>()), Times.Once);
    }

    /// <summary>
    /// Verifica que UpdateRefereeAsync actualiza árbitro y limpia caché
    /// </summary>
    [Fact]
    public async Task UpdateRefereeAsync_UpdatesRefereeAndClearsCache()
    {
        //Arrange
        var updateDto = new UpdateRefereeDto
        {
            FirstName = "Carlos",
            LastName = "Gomez",
            LicenseNumber = "LIC123",
        };
        var referee = new Referee { Id = 1, FirstName = "Carlos", LicenseNumber = "LIC123" };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(referee);
        _mockRepository.Setup(r => r.GetByLicenseNumberAsync("LIC123")).ReturnsAsync((Referee?)null);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Referee>())).ReturnsAsync(referee);

        //Act
        var result = await _service.UpdateRefereeAsync(1, updateDto);

        //Assert
        Assert.NotNull(result);
        _mockCache.Verify(c => c.RemoveAsync("referees:1"), Times.Once);
        _mockCache.Verify(c => c.RemoveAsync("referees:all"), Times.Once);
        _mockCache.Verify(c => c.RemoveAsync("referees:active"), Times.Once);
    }

    /// <summary>
    /// Verifica que DeleteRefereeAsync elimina árbitro y limpia caché
    /// </summary>
    [Fact]
    public async Task DeleteRefereeAsync_DeletesRefereeAndClearsCache()
    {
        //Arrange
        var referee = new Referee { Id = 1, FirstName = "Carlos", LicenseNumber = "LIC123" };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(referee);
        _mockRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        //Act
        var result = await _service.DeleteRefereeAsync(1);

        //Assert
        Assert.True(result);
        _mockCache.Verify(c => c.RemoveAsync("referees:1"), Times.Once);
        _mockCache.Verify(c => c.RemoveAsync("referees:all"), Times.Once);
        _mockCache.Verify(c => c.RemoveAsync("referees:active"), Times.Once);
    }
}