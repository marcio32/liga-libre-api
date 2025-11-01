namespace LigaLibre.WebMVC.Models
{
    public class StatisticsViewModel
    {
        public int TotalMatches { get; set; }
        public int FinishedMatches { get; set; }
        public int ScheduledMatches { get; set; }
        public int TotalGoals { get; set; }
        public double AverageGoalsPerMatch { get; set; }
        public int TotalClubs { get; set; }
        public List<TopScorerViewModel> TopScorers { get; set; } = new();
        public List<StandingViewModel> Standings { get; set; } = new();
    }

    public class TopScorerViewModel
    {
        public string PlayerName { get; set; } = string.Empty;
        public string ClubName { get; set; } = string.Empty;
        public int Goals { get; set; }
        public int Assists { get; set; }
    }

    public class StandingViewModel
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
}
