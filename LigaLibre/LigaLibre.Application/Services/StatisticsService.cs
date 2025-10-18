using LigaLibre.Application.DTOs;
using LigaLibre.Domain.Interfaces;
using LigaLibre.Domain.Enums;
using static LigaLibre.Application.DTOs.LeagueStatisticsDto;
using LigaLibre.Domain.Entities;
using LigaLibre.Application.Interfaces;
using Mapster;

namespace LigaLibre.Application.Services;

public class StatisticsService(IMatchRepository matchRepository, IPlayerRepository playerRepository, IClubRepository clubRepository, IRedisCacheService cacheService) : IStatisticsService
{
    /// <summary>
    /// Obtiene las estadísticas generales de la liga
    /// </summary>
    /// <returns>DTO con estadísticas completas de la liga</returns>
    public async Task<LeagueStatisticsDto> GetLeaguesStatisticsDtoAsync()
    {
        var cachedStats = await cacheService.GetAsync<LeagueStatisticsDto>("statistics:league");

        if (cachedStats != null) return cachedStats;

        var matches = await matchRepository.GetAllAsync();
        var players = await playerRepository.GetAllAsync();
        var clubs = await clubRepository.GetAllAsync();

        var finishedMatches = matches.Where(m => m.Status == MatchStatusEnum.Finished).ToList();
        var totalGoals = finishedMatches.Sum(m => (m.HomeScore ?? 0) + (m.AwayScore ?? 0));

        var topScorers = players
            .Where(p => p.Goals > 0)
            .OrderByDescending(p => p.Goals)
            .Take(10)
            .Select(p => p.Adapt<TopScorersDto>()).ToArray();

        var standings = GetClubStandingsAsync(clubs, matches);

        var leagueStats = new LeagueStatisticsDto
        {
            TotalMatches = matches.Count(),
            FinishedMatches = finishedMatches.Count(),
            ScheduledMatches = matches.Count(m => m.Status == MatchStatusEnum.Scheduled),
            TotalGoals = totalGoals,
            TotalClubs = clubs.Count(),
            TotalPlayers = players.Count(),
            TopScorers = topScorers,
            Standings = standings
        };

        if (leagueStats != null) await cacheService.SetAsync("statistics:league", leagueStats, TimeSpan.FromMinutes(5));

        return leagueStats == null ? new LeagueStatisticsDto() : leagueStats;
    }

    /// <summary>
    /// Obtiene las estadísticas de los partidos
    /// </summary>
    /// <returns>DTO con estadísticas de partidos</returns>
    public async Task<MatchStatisticsDto> GetMatchesStatisticsDtoAsync()
    {
        var cachedStats = await cacheService.GetAsync<MatchStatisticsDto>("statistics:matches");

        if (cachedStats != null) return cachedStats;

        var matches = await matchRepository.GetAllAsync();
        var recentMatches = matches
            .Where(m => m.Status == MatchStatusEnum.Finished)
            .OrderByDescending(m => m.MatchDate)
            .Take(10)
            .Select(m => m.Adapt<RecentMatchDto>()).ToArray();

        var totalMatches = matches.Count();
        var finishedMatches = matches.Count(m => m.Status == MatchStatusEnum.Finished);

        var matchStats = new MatchStatisticsDto
        {
            TotalMatches = totalMatches,
            FinishedMatches = finishedMatches,
            ScheduledMatches = matches.Count(m => m.Status == MatchStatusEnum.Scheduled),
            InProgressMatches = matches.Count(m => m.Status == MatchStatusEnum.Inprogress),
            PostponedMatches = matches.Count(m => m.Status == MatchStatusEnum.Postponed),
            CancelledMatches = matches.Count(m => m.Status == MatchStatusEnum.Cancelled),
            CompletionPercentage = totalMatches > 0 ? (double)finishedMatches / totalMatches * 100 : 0,
            RecentMatches = recentMatches
        };

        if (matchStats != null) await cacheService.SetAsync("statistics:matches", matchStats, TimeSpan.FromMinutes(5));

        return matchStats == null ? new MatchStatisticsDto() : matchStats;
    }


    /// <summary>
    /// Obtiene las estadísticas de los jugadores
    /// </summary>
    /// <returns>DTO con estadísticas de jugadores</returns>
    public async Task<PlayerStatisticsDto> GetPlayersStatisticsDtoAsync()
    {
        var cachedStats = await cacheService.GetAsync<PlayerStatisticsDto>("statistics:players");

        if (cachedStats != null) return cachedStats;

        var players = await playerRepository.GetAllAsync();

        var topScorers = players
            .Where(p => p.Goals > 0)
            .OrderByDescending(p => p.Goals)
            .Take(10)
            .Select(p => p.Adapt<TopScorersDto>()).ToArray();

        var topAssists = players
            .Where(p => p.Assists > 0)
            .OrderByDescending(p => p.Assists)
            .Take(10)
            .Select(p => p.Adapt<TopAssistsDto>()).ToArray();

        var positionStats = players
            .GroupBy(p => p.Position)
            .Select(g => g.Adapt<PositionStatsDto>()).ToArray();

        var playerStats = new PlayerStatisticsDto
        {
            TotalPlayers = players.Count(),
            ActivePlayers = players.Count(p => p.IsActive),
            AverageAge = players.Any() ? players.Average(p => p.Age) : 0,
            TopScorers = topScorers,
            TopAssists = topAssists,
            PositionStats = positionStats
        };

        if (playerStats != null) await cacheService.SetAsync("statistics:players", playerStats, TimeSpan.FromMinutes(5));

        return playerStats == null ? new PlayerStatisticsDto() : playerStats;
    }

    public static ClubStadingsDto[] GetClubStandingsAsync(IEnumerable<Club> clubs, IEnumerable<Match> matches)
    {
        return clubs.Select(club =>
        {
            var homeMatches = matches.Where(m => m.HomeClubId == club.Id && m.Status == MatchStatusEnum.Finished);
            var awayMatches = matches.Where(m => m.AwayClubId == club.Id && m.Status == MatchStatusEnum.Finished);

            var wins = homeMatches.Count(m => m.HomeScore > m.AwayScore) +
                       awayMatches.Count(m => m.AwayScore > m.HomeScore);

            var draws = homeMatches.Count(m => m.HomeScore == m.AwayScore) +
                      awayMatches.Count(m => m.AwayScore == m.HomeScore);

            var losses = homeMatches.Count(m => m.HomeScore < m.AwayScore) +
                       awayMatches.Count(m => m.AwayScore < m.HomeScore);

            var goalsFor = homeMatches.Sum(m => m.HomeScore ?? 0) +
                           awayMatches.Sum(m => m.AwayScore ?? 0);

            var goalsAgainst = homeMatches.Sum(m => m.AwayScore ?? 0) +
                               awayMatches.Sum(m => m.HomeScore ?? 0);

            return new ClubStadingsDto
            {
                ClubName = club.Name,
                MatchesPlayed = wins + draws + losses,
                Wins = wins,
                Draws = draws,
                Losses = losses,
                GoalsFor = goalsFor,
                GoalsAgainst = goalsAgainst,
                GoalsDifference = goalsFor - goalsAgainst,
                Points = wins * 3 + draws
            };
        })
         .OrderByDescending(s => s.Points)
         .ThenByDescending(s => s.GoalsDifference)
         .ThenByDescending(s => s.GoalsFor)
         .ToArray();
    }
}

