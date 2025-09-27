using LigaLibre.Domain.Entities;
using LigaLibre.Domain.Enums;

namespace LigaLibre.Domain.Interfaces;

public interface IMatchRepository
{
    Task<Match> CreateAsync(Match match);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<Match>> GetAllAsync();
    Task<IEnumerable<Match>> GetByClubAsync(int clubId);
    Task<Match?> GetByIdAsync(int id);
    Task<IEnumerable<Match>> GetByRoundAsync(int round);
    Task<IEnumerable<Match>> GetByStatusAsync(MatchStatusEnum status);
    Task<Match?> UpdateAsync(Match match);
}
