
using LigaLibre.Domain.Enums;

namespace LigaLibre.Domain.Entities;

public class Referee
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public RefereeCategoryEnum Category { get; set; } = RefereeCategoryEnum.National;
    public ICollection<Match> Matches { get; set; } = new List<Match>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

