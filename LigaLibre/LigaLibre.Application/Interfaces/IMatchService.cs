using LigaLibre.Application.DTOs;

namespace LigaLibre.Application.Interfaces;

public interface IMatchService
{
    Task<IEnumerable<MatchDto>> GetAllMatchesAsync();
    Task<MatchDto?> GetMatchByIdAsync(int id);
    Task<IEnumerable<MatchDto>> GetMatchesByClubAsync(int clubId);
    Task<IEnumerable<MatchDto>> GetMatchesByRoundAsync(int round);
    Task<MatchDto> CreateMatchAsync(CreateMatchDto matchDto);
    Task<MatchDto?> UpdateMatchAsync(UpdateMatchDto matchDto);
    Task<bool> DeleteMatchAsync(int id);
}