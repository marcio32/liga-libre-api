namespace LigaLibre.Application.DTOs;

public class LeagueStatisticsDto
{
    public int TotalMatches { get; set; }
    public int FinishedMatches { get; set; }
    public int ScheduledMatches { get; set; }
    public int TotalGoals { get; set; }
    public double AverageGoalsPerMatch { get; set; }
    public int TotalClubs { get; set; }
    public int TotalPlayers { get; set; }
    public TopScorersDto[] TopScorers { get; set; } = Array.Empty<TopScorersDto>();
    public ClubStadingsDto[] Standings { get; set; } = Array.Empty<ClubStadingsDto>();


    public class TopScorersDto
    {
        public string PlayerName { get; set; } = string.Empty;
        public string ClubName { get; set; } = string.Empty;
        public int Goals { get; set; }
        public int Assists { get; set; }
        public int MatchesPlayed { get; set; }
    }

    public class ClubStadingsDto
    {
        public string ClubName { get; set; } = string.Empty;
        public int MatchesPlayed { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
        public int GoalsDifference { get; set; }
        public int Points { get; set; }
    }

    public class MatchStatisticsDto
    {
        public int TotalMatches { get; set; }
        public int FinishedMatches { get; set; }
        public int ScheduledMatches { get; set; }
        public int InProgressMatches { get; set; }
        public int PostponedMatches { get; set; }
        public int CancelledMatches { get; set; }
        public double CompletionPercentage { get; set; }
        public RecentMatchDto[] RecentMatches { get; set; } = Array.Empty<RecentMatchDto>();
    }

    public class RecentMatchDto
    { 
        public string HomeClub { get; set; } = string.Empty ;
        public string AwayClub { get; set; } = string.Empty;
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public DateTime MatchDate { get; set; }
        public string status { get; set; } = string.Empty;
    }

    public class PlayerStatisticsDto
    {
        public int TotalPlayers { get; set; }
        public int ActivePlayers { get; set; }
        public double AverageAge { get; set; }
        public TopScorersDto[] TopScorers { get; set; } = Array.Empty<TopScorersDto>();
        public TopAssistsDto[] TopAssists { get; set; } = Array.Empty<TopAssistsDto>();
        public PositionStatsDto[] PositionStats { get; set; } = Array.Empty<PositionStatsDto>();
    }

    public class TopAssistsDto
    {
        public string PlayerName { get; set; } = string.Empty;
        public string ClubName { get; set; } = string.Empty;
        public int Assists { get; set; }
        public int Goals { get; set; }
    }

    public class PositionStatsDto
    {
        public string Position { get; set; } = string.Empty;
        public int playerCount { get; set; }
        public double AverageAge { get; set; }
        public int TotalGoals { get; set; }
    }
}

