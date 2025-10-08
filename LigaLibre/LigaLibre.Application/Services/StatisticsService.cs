using LigaLibre.Application.DTOs;
using LigaLibre.Domain.Interfaces;
using LigaLibre.Domain.Enums;
using static LigaLibre.Application.DTOs.LeagueStatisticsDto;
using LigaLibre.Domain.Entities;
using LigaLibre.Application.Interfaces;

namespace LigaLibre.Application.Services;

public class StatisticsService(IMatchRepository matchRepository, IPlayerRepository playerRepository, IClubRepository clubRepository) : IStatisticsService
{
    public async Task<LeagueStatisticsDto> GetLeaguesStatisticsDtoAsync()
    {
        var matches = await matchRepository.GetAllAsync();
        var players = await playerRepository.GetAllAsync();
        var clubs = await clubRepository.GetAllAsync();

        var finishedMatches = matches.Where(m => m.Status == MatchStatusEnum.Finished).ToList();
        var totalGoals = finishedMatches.Sum(m => (m.HomeScore ?? 0) + (m.AwayScore ?? 0));

        var topScorers = players
            .Where(p => p.Goals > 0)
            .OrderByDescending(p => p.Goals)
            .Take(10)
            .Select(p => new TopScorersDto
            {
                PlayerName = $"{p.FirstName} {p.LastName}",
                ClubName = p.Club?.Name ?? "Sin Club",
                Goals = p.Goals,
                Assists = p.Assists,
                MatchesPlayed = p.MatchesPlayed
            }).ToArray();

        var standings = GetClubStandingsAsync(clubs, matches);

        return new LeagueStatisticsDto
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
    }

    public async Task<MatchStatisticsDto> GetMatchesStatisticsDtoAsync()
    {
        var matches = await matchRepository.GetAllAsync();
        var recentMatches = matches
            .Where(m => m.Status == MatchStatusEnum.Finished)
            .OrderByDescending(m => m.MatchDate)
            .Take(10)
            .Select(m => new RecentMatchDto
            {
                HomeClub = m.HomeClub?.Name ?? "TBD",
                AwayClub = m.AwayClub?.Name ?? "TBD",
                HomeScore = m.HomeScore,
                AwayScore = m.AwayScore,
                MatchDate = m.MatchDate,
                status = m.Status.ToString()
            }).ToArray();

        var totalMatches = matches.Count();
        var finishedMatches = matches.Count(m => m.Status == MatchStatusEnum.Finished);

        return new MatchStatisticsDto
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
    }


    public async Task<PlayerStatisticsDto> GetPlayersStatisticsDtoAsync()
    {
        var players = await playerRepository.GetAllAsync();

        var topScorers = players
            .Where(p => p.Goals > 0)
            .OrderByDescending(p => p.Goals)
            .Take(10)
            .Select(p => new TopScorersDto
            {
                PlayerName = $"{p.FirstName} {p.LastName}",
                ClubName = p.Club?.Name ?? "Sin Club",
                Goals = p.Goals,
                Assists = p.Assists,
                MatchesPlayed = p.MatchesPlayed
            }).ToArray();

        var topAssists = players
            .Where(p => p.Assists > 0)
            .OrderByDescending(p => p.Assists)
            .Take(10)
            .Select(p => new TopAssistsDto
            {
                PlayerName = $"{p.FirstName} {p.LastName}",
                ClubName = p.Club?.Name ?? "Sin Club",
                Goals = p.Goals,
                Assists = p.Assists,
            }).ToArray();

        var positionStats = players
            .GroupBy(p => p.Position)
            .Select(g => new PositionStatsDto
            {
                Position = g.Key,
                playerCount = g.Count(),
                AverageAge = g.Average(p => p.Age),
                TotalGoals = g.Sum(p => p.Goals),
            }).ToArray();

        return new PlayerStatisticsDto
        {
            TotalPlayers = players.Count(),
            ActivePlayers = players.Count(p => p.IsActive),
            AverageAge = players.Any() ? players.Average(p => p.Age) : 0,
            TopScorers = topScorers,
            TopAssists = topAssists,
            PositionStats = positionStats
        };
    }

    public static ClubStadingsDto[] GetClubStandingsAsync(IEnumerable<Club> clubs, IEnumerable<Match> matches)
    {
        var standings = clubs.Select(club =>
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

        return standings;
    }
}

