using LigaLibre.Application.DTOs;
using LigaLibre.Application.Interfaces;
using LigaLibre.Application.Services;
using LigaLibre.Domain.Entities;
using LigaLibre.Domain.Interfaces;
using Moq;

namespace LigaLibre.Tests.Services;

public class PlayerServiceTests
{
    private readonly Mock<IPlayerRepository> _mockRepository;
    private readonly Mock<IRedisCacheService> _mockCache;
    private readonly Mock<ISqsService> _mockSqs;
    private readonly PlayerService _service;

    public PlayerServiceTests()
    {
        _mockRepository = new Mock<IPlayerRepository>();
        _mockCache = new Mock<IRedisCacheService>();
        _mockSqs = new Mock<ISqsService>();
        _service = new PlayerService(_mockRepository.Object, _mockCache.Object, _mockSqs.Object);
    }

    /// <summary>
    /// Verifica que GetAllPlayers retorna jugadores desde caché
    /// </summary>
    [Fact]
    public async Task GetAllPlayers_ReturnsCachedPlayers()
    {
        //Arrange
        var players = new List<PlayerDto> { new PlayerDto { Id = 1, FirstName = "Juan" } };
        _mockCache.Setup(c => c.GetAsync<IEnumerable<PlayerDto>>("players:all")).ReturnsAsync(players);

        //Act
        var result = await _service.GetAllPlayers();

        //Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    /// <summary>
    /// Verifica que GetAllPlayers consulta repositorio cuando no hay caché
    /// </summary>
    [Fact]
    public async Task GetAllPlayers_NoCacheReturnsFromRepository()
    {
        //Arrange
        var players = new List<Player> { new Player { Id = 1, FirstName = "Juan" } };
        _mockCache.Setup(c => c.GetAsync<IEnumerable<PlayerDto>>("players:all")).ReturnsAsync((IEnumerable<PlayerDto>?)null);
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(players);

        //Act
        var result = await _service.GetAllPlayers();

        //Assert
        Assert.NotNull(result);
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    /// <summary>
    /// Verifica que GetPlayerByIdAsync retorna jugador desde caché
    /// </summary>
    [Fact]
    public async Task GetPlayerByIdAsync_ReturnsCachedPlayer()
    {
        //Arrange
        var player = new PlayerDto { Id = 1, FirstName = "Juan" };
        _mockCache.Setup(c => c.GetAsync<PlayerDto>("players:1")).ReturnsAsync(player);

        //Act
        var result = await _service.GetPlayerByIdAsync(1);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    /// <summary>
    /// Verifica que CreatePlayerAsync crea jugador y envía mensaje SQS
    /// </summary>
    [Fact]
    public async Task CreatePlayerAsync_CreatesPlayerAndSendsMessage()
    {
        //Arrange
        var createDto = new CreatePlayerDto
        {
            FirstName = "Juan",
            LastName = "Perez",
            Age = 25,
            Position = "Delantero",
            JerseyNumber = 10,
            Height = 1.75m,
            Weight = 75,
            DateOfBirth = DateTime.Now.AddYears(-25),
            ClubId = 1
        };
        var player = new Player { Id = 1, FirstName = "Juan" };
        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Player>())).ReturnsAsync(player);

        //Act
        var result = await _service.CreatePlayerAsync(createDto);

        //Assert
        Assert.NotNull(result);
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Player>()), Times.Once);
    }

    /// <summary>
    /// Verifica que UpdatePlayerAsync actualiza jugador y limpia caché
    /// </summary>
    [Fact]
    public async Task UpdatePlayerAsync_UpdatesPlayerAndClearsCache()
    {
        //Arrange
        var updateDto = new UpdatePlayerDto
        {
            FirstName = "Juan",
            LastName = "Perez",
            Age = 26,
            Position = "Delantero",
            JerseyNumber = 10,
            Height = 1.75m,
            Weight = 75,
            DateOfBirth = DateTime.Now.AddYears(-26),
            ClubId = 1
        };
        var player = new Player { Id = 1, FirstName = "Juan", ClubId = 1 };
        var updatedPlayer = new Player { Id = 1, FirstName = "Juan", ClubId = 1 };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(player);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Player>())).ReturnsAsync(updatedPlayer);

        //Act
        var result = await _service.UpdatePlayerAsync(1, updateDto);

        //Assert
        Assert.NotNull(result);
        _mockCache.Verify(c => c.RemoveAsync("players:all"), Times.Once);
        _mockCache.Verify(c => c.RemoveAsync("players:1"), Times.Once);
    }

    /// <summary>
    /// Verifica que DeletePlayerAsync elimina jugador y limpia caché
    /// </summary>
    [Fact]
    public async Task DeletePlayerAsync_DeletesPlayerAndClearsCache()
    {
        //Arrange
        var player = new Player { Id = 1, FirstName = "Juan", ClubId = 1 };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(player);
        _mockRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        //Act
        var result = await _service.DeletePlayerAsync(1);

        //Assert
        Assert.True(result);
        _mockCache.Verify(c => c.RemoveAsync("players:all"), Times.Once);
        _mockCache.Verify(c => c.RemoveAsync("players:1"), Times.Once);
    }
}