using LigaLibre.Application.DTOs;
using LigaLibre.Application.Services;
using LigaLibre.Domain.Entities;
using LigaLibre.Domain.Interfaces;
using Moq;
using static LigaLibre.Application.DTOs.LeagueStatisticsDto;
using Match = LigaLibre.Domain.Entities.Match;

namespace LigaLibre.Tests.Services;

public class StatisticsServiceTests
{
    private readonly Mock<IMatchRepository> _mockMatchRepository;
    private readonly Mock<IPlayerRepository> _mockPlayerRepository;
    private readonly Mock<IClubRepository> _mockClubRepository;
    private readonly Mock<IRedisCacheService> _mockCache;
    private readonly StatisticsService _service;

    public StatisticsServiceTests()
    {
        _mockMatchRepository = new Mock<IMatchRepository>();
        _mockPlayerRepository = new Mock<IPlayerRepository>();
        _mockClubRepository = new Mock<IClubRepository>();
        _mockCache = new Mock<IRedisCacheService>();
        _service = new StatisticsService(_mockMatchRepository.Object, _mockPlayerRepository.Object, 
            _mockClubRepository.Object, _mockCache.Object);
    }

    /// <summary>
    /// Verifica que GetLeaguesStatisticsDtoAsync retorna estadísticas desde caché
    /// </summary>
    [Fact]
    public async Task GetLeaguesStatisticsDtoAsync_ReturnsCachedStatistics()
    {
        //Arrange
        var stats = new LeagueStatisticsDto { TotalMatches = 10, TotalClubs = 5 };
        _mockCache.Setup(c => c.GetAsync<LeagueStatisticsDto>("statistics:league")).ReturnsAsync(stats);

        //Act
        var result = await _service.GetLeaguesStatisticsDtoAsync();

        //Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.TotalMatches);
        Assert.Equal(5, result.TotalClubs);
    }

    /// <summary>
    /// Verifica que GetLeaguesStatisticsDtoAsync calcula estadísticas cuando no hay caché
    /// </summary>
    [Fact]
    public async Task GetLeaguesStatisticsDtoAsync_CalculatesWhenNoCache()
    {
        //Arrange
        var matches = new List<Match> { new Match { Id = 1 }, new Match { Id = 2 } };
        var clubs = new List<Club> { new Club { Id = 1 }, new Club { Id = 2 } };
        var players = new List<Player> { new Player { Id = 1 }, new Player { Id = 2 } };

        _mockCache.Setup(c => c.GetAsync<LeagueStatisticsDto>("statistics:league")).ReturnsAsync((LeagueStatisticsDto?)null);
        _mockMatchRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(matches);
        _mockClubRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(clubs);
        _mockPlayerRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(players);

        //Act
        var result = await _service.GetLeaguesStatisticsDtoAsync();

        //Assert
        Assert.NotNull(result);
        _mockMatchRepository.Verify(r => r.GetAllAsync(), Times.Once);
        _mockClubRepository.Verify(r => r.GetAllAsync(), Times.Once);
        _mockPlayerRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    /// <summary>
    /// Verifica que GetMatchesStatisticsDtoAsync retorna estadísticas desde caché
    /// </summary>
    [Fact]
    public async Task GetMatchesStatisticsDtoAsync_ReturnsCachedStatistics()
    {
        //Arrange
        var stats = new MatchStatisticsDto { TotalMatches = 20 };
        _mockCache.Setup(c => c.GetAsync<MatchStatisticsDto>("statistics:matches")).ReturnsAsync(stats);

        //Act
        var result = await _service.GetMatchesStatisticsDtoAsync();

        //Assert
        Assert.NotNull(result);
        Assert.Equal(20, result.TotalMatches);
    }

    /// <summary>
    /// Verifica que GetPlayersStatisticsDtoAsync retorna estadísticas desde caché
    /// </summary>
    [Fact]
    public async Task GetPlayersStatisticsDtoAsync_ReturnsCachedStatistics()
    {
        //Arrange
        var stats = new PlayerStatisticsDto { TotalPlayers = 50 };
        _mockCache.Setup(c => c.GetAsync<PlayerStatisticsDto>("statistics:players")).ReturnsAsync(stats);

        //Act
        var result = await _service.GetPlayersStatisticsDtoAsync();

        //Assert
        Assert.NotNull(result);
        Assert.Equal(50, result.TotalPlayers);
    }
}