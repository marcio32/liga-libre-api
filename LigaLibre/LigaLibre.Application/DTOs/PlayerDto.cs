namespace LigaLibre.Application.DTOs;

public class PlayerDto
{
    public int Id { get; set; }
    public int Age { get; set; }
    public int JerseuNumber { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public decimal Height { get; set; }
    public decimal Weight { get; set; }
    public int Goals { get; set; } = 0;
    public int Assists { get; set; } = 0;
    public int YellowCards { get; set; } = 0;
    public int RedCards { get; set; } = 0;
    public int MatchesPlayed { get; set; } = 0;
    public int ClubId { get; set; }
    public bool IsActive { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime JoinedClubDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
public class CreatePlayerDto
{
    public int Age { get; set; }
    public int JerseyNumber { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public decimal Height { get; set; }
    public decimal Weight { get; set; }
    public int ClubId { get; set; }
    public DateTime DateOfBirth { get; set; }
}

public class UpdatePlayerDto
{
    public int Id { get; set; }
    public int Age { get; set; }
    public int JerseyNumber { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public decimal Height { get; set; }
    public decimal Weight { get; set; }
    public int ClubId { get; set; }
    public DateTime DateOfBirth { get; set; }
}

