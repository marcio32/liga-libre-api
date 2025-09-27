using LigaLibre.Domain.Enums;

namespace LigaLibre.Domain.Entities;

public class Match
{
    public int Id { get; set; }
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }
    public int Round { get; set; }
    public int HomeClubId { get; set; }
    public int AwayClubId { get; set; }
    public string Stadium { get; set; } = string.Empty;
    public string?  Notes { get; set; }
    public int? RefereeId { get; set; }
    public Referee? Referee { get; set; }
    public Club HomeClub { get; set; } = null!;
    public Club AwayClub { get; set; } = null!;
    public MatchStatusEnum Status { get; set; } = MatchStatusEnum.Scheduled;
    public DateTime MatchDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
