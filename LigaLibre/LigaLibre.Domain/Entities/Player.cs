using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LigaLibre.Domain.Entities;

public class Player
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
    public Club Club { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime JoinedClubDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


}

