using LigaLibre.Domain.Enums;

namespace LigaLibre.Application.DTOs;

public class MatchDto
{
    public int Id { get; set; }
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }
    public int Round { get; set; }
    public int HomeClubId { get; set; }
    public int AwayClubId { get; set; }
    public int Stadium { get; set; }
    public int? RefereeId { get; set; }
    public string? RefereeName { get; set; }
    public string HomeClubName { get; set; } = string.Empty;
    public string AwayClubName { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string Result => HomeScore.HasValue && AwayScore.HasValue ? $"{HomeScore} - {AwayScore}" : "vs";
    public DateTime MatchDate { get; set; }
    public MatchStatusEnum Status { get; set; }

}

public class CreateMatchDto
{
    public int Round { get; set; }
    public int HomeClubId { get; set; }
    public int AwayClubId { get; set; }
    public int Stadium { get; set; }
    public int? RefereeId { get; set; }
    public string? Notes { get; set; }
    public DateTime MatchDate { get; set; }
}

public class UpdateMatchDto
{
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
    public int Round { get; set; }
    public int HomeClubId { get; set; }
    public int AwayClubId { get; set; }
    public int Stadium { get; set; }
    public int? RefereeId { get; set; }
    public string? Notes { get; set; }
    public MatchStatusEnum Status { get; set; } 
    public DateTime MatchDate { get; set; }
}