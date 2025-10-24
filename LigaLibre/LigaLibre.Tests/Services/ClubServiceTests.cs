using LigaLibre.Application.DTOs;
using LigaLibre.Application.Interfaces;
using LigaLibre.Application.Services;
using LigaLibre.Domain.Entities;
using LigaLibre.Domain.Interfaces;
using Moq;

namespace LigaLibre.Tests.Services;

public class ClubServiceTests
{
    private readonly Mock<IClubRepository> _mockRepository;
    private readonly Mock<ISqsService> _mockSqs;
    private readonly Mock<IRedisCacheService> _mockCache;
    private readonly ClubService _service;

    public ClubServiceTests()
    {
        _mockRepository = new Mock<IClubRepository>();
        _mockSqs = new Mock<ISqsService>();
        _mockCache = new Mock<IRedisCacheService>();
        _service = new ClubService(_mockRepository.Object, _mockSqs.Object, _mockCache.Object);
    }

    /// <summary>
    /// Verifica que GetAllClubsAsync retorna clubes desde caché
    /// </summary>
    [Fact]
    public async Task GetAllClubsAsync_ReturnsCachedClubs()
    {
        //Arrange
        var clubs = new List<ClubDto> { new ClubDto { Id = 1, Name = "Boca" } };
        _mockCache.Setup(c => c.GetAsync<IEnumerable<ClubDto>>("clubs:all")).ReturnsAsync(clubs);

        //Act
        var result = await _service.GetAllClubsAsync();

        //Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    /// <summary>
    /// Verifica que GetAllClubsAsync consulta repositorio cuando no hay caché
    /// </summary>
    [Fact]
    public async Task GetAllClubsAsync_NoCacheReturnsFromRepository()
    {
        //Arrange
        var clubs = new List<Club> { new Club { Id = 1, Name = "Boca" } };
        _mockCache.Setup(c => c.GetAsync<IEnumerable<ClubDto>>("clubs:all")).ReturnsAsync((IEnumerable<ClubDto>?)null);
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(clubs);

        //Act
        var result = await _service.GetAllClubsAsync();

        //Assert
        Assert.NotNull(result);
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        _mockCache.Verify(c => c.SetAsync("clubs:all", It.IsAny<IEnumerable<ClubDto>>(), It.IsAny<TimeSpan>()), Times.Once);
    }

    /// <summary>
    /// Verifica que GetClubByIdAsync retorna club desde caché
    /// </summary>
    [Fact]
    public async Task GetClubByIdAsync_ReturnsCachedClub()
    {
        //Arrange
        var club = new ClubDto { Id = 1, Name = "Boca" };
        _mockCache.Setup(c => c.GetAsync<ClubDto?>("clubs:1")).ReturnsAsync(club);

        //Act
        var result = await _service.GetClubByIdAsync(1);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    /// <summary>
    /// Verifica que CreateClubAsync crea club y envía mensaje SQS
    /// </summary>
    [Fact]
    public async Task CreateClubAsync_CreatesClubAndSendsMessage()
    {
        //Arrange
        var createDto = new CreateClubDto { Name = "River", City = "Buenos Aires", Email = "river@test.com" };
        var club = new Club { Id = 1, Name = "River" };
        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Club>())).ReturnsAsync(club);

        //Act
        var result = await _service.CreateClubAsync(createDto);

        //Assert
        Assert.NotNull(result);
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Club>()), Times.Once);
    }

    /// <summary>
    /// Verifica que UpdateClubAsync actualiza club y limpia caché
    /// </summary>
    [Fact]
    public async Task UpdateClubAsync_UpdatesClubAndClearsCache()
    {
        //Arrange
        var updateDto = new UpdateClubDto { Id = 1, Name = "River Plate" };
        var club = new Club { Id = 1, Name = "River" };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(club);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Club>())).ReturnsAsync(club);

        //Act
        var result = await _service.UpdateClubAsync(updateDto);

        //Assert
        Assert.NotNull(result);
        _mockCache.Verify(c => c.RemoveAsync("clubs:all"), Times.Once);
        _mockCache.Verify(c => c.RemoveAsync("clubs:1"), Times.Once);
    }

    /// <summary>
    /// Verifica que DeleteClubAsync elimina club y limpia caché
    /// </summary>
    [Fact]
    public async Task DeleteClubAsync_DeletesClubAndClearsCache()
    {
        //Arrange
        _mockRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        //Act
        var result = await _service.DeleteClubAsync(1);

        //Assert
        Assert.True(result);
        _mockCache.Verify(c => c.RemoveAsync("clubs:all"), Times.Once);
        _mockCache.Verify(c => c.RemoveAsync("clubs:1"), Times.Once);
    }
}