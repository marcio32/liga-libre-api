namespace LigaLibre.Application.DTOs;

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

