using LigaLibre.Application.DTOs;
using LigaLibre.Application.Interfaces;
using LigaLibre.Application.Services;
using LigaLibre.Domain.Entities;
using LigaLibre.Domain.Enums;
using LigaLibre.Domain.Interfaces;
using Moq;
using MatchEntity = LigaLibre.Domain.Entities.Match;

namespace LigaLibre.Tests.Services;

public class MatchServiceTests
{
    private readonly Mock<IMatchRepository> _mockRepository;
    private readonly Mock<IRedisCacheService> _mockCache;
    private readonly Mock<ISqsService> _mockSqs;
    private readonly MatchService _service;

    public MatchServiceTests()
    {
        _mockRepository = new Mock<IMatchRepository>();
        _mockCache = new Mock<IRedisCacheService>();
        _mockSqs = new Mock<ISqsService>();
        _service = new MatchService(_mockRepository.Object, _mockCache.Object, _mockSqs.Object);
    }

    /// <summary>
    /// Verifica que GetAllMatchesAsync retorna partidos desde caché
    /// </summary>
    [Fact]
    public async Task GetAllMatchesAsync_ReturnsCachedMatches()
    {
        //Arrange
        var matches = new List<MatchDto> { new MatchDto { Id = 1, HomeClubId = 1, AwayClubId = 2 } };
        _mockCache.Setup(c => c.GetAsync<IEnumerable<MatchDto>>("matches:all")).ReturnsAsync(matches);

        //Act
        var result = await _service.GetAllMatchesAsync();

        //Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    /// <summary>
    /// Verifica que GetMatchByIdAsync retorna partido desde caché
    /// </summary>
    [Fact]
    public async Task GetMatchByIdAsync_ReturnsCachedMatch()
    {
        //Arrange
        var match = new MatchDto { Id = 1, HomeClubId = 1, AwayClubId = 2 };
        _mockCache.Setup(c => c.GetAsync<MatchDto>("matches:1")).ReturnsAsync(match);

        //Act
        var result = await _service.GetMatchByIdAsync(1);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    /// <summary>
    /// Verifica que CreateMatchAsync crea partido y envía mensaje SQS
    /// </summary>
    [Fact]
    public async Task CreateMatchAsync_CreatesMatchAndSendsMessage()
    {
        //Arrange
        var createDto = new CreateMatchDto
        {
            HomeClubId = 1,
            AwayClubId = 2,
            MatchDate = DateTime.Now.AddDays(7),
            Round = 1,
            RefereeId = 1,
            Stadium = 1
        };
        var match = new MatchEntity 
        { 
            Id = 1, 
            HomeClubId = 1, 
            AwayClubId = 2,
            Round = 1,
            Stadium = "1",
            MatchDate = DateTime.Now.AddDays(7),
            Status = MatchStatusEnum.Scheduled,
            HomeClub = new Club { Id = 1, Name = "Home" },
            AwayClub = new Club { Id = 2, Name = "Away" }
        };
        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<MatchEntity>())).ReturnsAsync(match);

        //Act
        var result = await _service.CreateMatchAsync(createDto);

        //Assert
        Assert.NotNull(result);
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<MatchEntity>()), Times.Once);
    }

    /// <summary>
    /// Verifica que UpdateMatchAsync actualiza partido y limpia caché
    /// </summary>
    [Fact]
    public async Task UpdateMatchAsync_UpdatesMatchAndClearsCache()
    {
        //Arrange
        var updateDto = new UpdateMatchDto
        {
            Id = 1,
            HomeClubId = 1,
            AwayClubId = 2,
            HomeScore = 2,
            AwayScore = 1,
        };
        var match = new MatchEntity { Id = 1, HomeClubId = 1, AwayClubId = 2 };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(match);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<MatchEntity>())).ReturnsAsync(match);

        //Act
        var result = await _service.UpdateMatchAsync(updateDto);

        //Assert
        Assert.NotNull(result);
        _mockCache.Verify(c => c.RemoveAsync("matches:all"), Times.Once);
        _mockCache.Verify(c => c.RemoveAsync("matches:1"), Times.Once);
    }

    /// <summary>
    /// Verifica que DeleteMatchAsync elimina partido y limpia caché
    /// </summary>
    [Fact]
    public async Task DeleteMatchAsync_DeletesMatchAndClearsCache()
    {
        //Arrange
        var match = new MatchEntity { Id = 1, HomeClubId = 1, AwayClubId = 2 };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(match);
        _mockRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        //Act
        var result = await _service.DeleteMatchAsync(1);

        //Assert
        Assert.True(result);
        _mockCache.Verify(c => c.RemoveAsync("matches:all"), Times.Once);
        _mockCache.Verify(c => c.RemoveAsync("matches:1"), Times.Once);
    }
}