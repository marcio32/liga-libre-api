using LigaLibre.Application.DTOs;
using LigaLibre.Application.Interfaces;
using LigaLibre.Domain.Entities;
using LigaLibre.Domain.Interfaces;

namespace LigaLibre.Application.Services;

public class MatchService(IMatchRepository matchRepository) : IMatchService
{
    public async Task<IEnumerable<MatchDto>> GetAllMatchesAsync()
    {
        var matches = await matchRepository.GetAllAsync();
        return matches.Select(MapToDto);
    }

    public async Task<MatchDto?> GetMatchByIdAsync(int id)
    {
        var match = await matchRepository.GetByIdAsync(id);
        return match != null ? MapToDto(match) : null;
    }

    public async Task<IEnumerable<MatchDto>> GetMatchesByClubAsync(int clubId)
    {
        var matches = await matchRepository.GetByClubAsync(clubId);
        return matches.Select(MapToDto);
    }

    public async Task<IEnumerable<MatchDto>> GetMatchesByRoundAsync(int round)
    {
        var matches = await matchRepository.GetByRoundAsync(round);
        return matches.Select(MapToDto);
    }

    public async Task<MatchDto> CreateMatchAsync(CreateMatchDto matchDto)
    {
        var match = new Match
        {
            Round = matchDto.Round,
            HomeClubId = matchDto.HomeClubId,
            AwayClubId = matchDto.AwayClubId,
            Stadium = matchDto.Stadium.ToString(),
            RefereeId = matchDto.RefereeId,
            Notes = matchDto.Notes,
            MatchDate = matchDto.MatchDate,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        var createdMatch = await matchRepository.CreateAsync(match);
        return MapToDto(createdMatch);
    }

    public async Task<MatchDto?> UpdateMatchAsync(int id, UpdateMatchDto matchDto)
    {
        var existingMatch = await matchRepository.GetByIdAsync(id);
        if (existingMatch == null) return null;

        existingMatch.HomeScore = matchDto.HomeScore;
        existingMatch.AwayScore = matchDto.AwayScore;
        existingMatch.Round = matchDto.Round;
        existingMatch.HomeClubId = matchDto.HomeClubId;
        existingMatch.AwayClubId = matchDto.AwayClubId;
        existingMatch.Stadium = matchDto.Stadium.ToString();
        existingMatch.RefereeId = matchDto.RefereeId;
        existingMatch.Notes = matchDto.Notes;
        existingMatch.Status = matchDto.Status;
        existingMatch.MatchDate = matchDto.MatchDate;
        existingMatch.UpdatedAt = DateTime.UtcNow;

        var updatedMatch = await matchRepository.UpdateAsync(existingMatch);
        return updatedMatch != null ? MapToDto(updatedMatch) : null;
    }

    public async Task<bool> DeleteMatchAsync(int id)
    {
        return await matchRepository.DeleteAsync(id);
    }

    private static MatchDto MapToDto(Match match) => new MatchDto
    {
        Id = match.Id,
        HomeClubId = match.HomeClubId,
        HomeClubName = match.HomeClub?.Name ?? string.Empty,
        AwayClubId = match.AwayClubId,
        AwayClubName = match.AwayClub?.Name ?? string.Empty,
        RefereeId = match.RefereeId,
        RefereeName = match.Referee != null ? $"{match.Referee.FirstName} {match.Referee.LastName}" : null,
        MatchDate = match.MatchDate,
        Round = match.Round,
        HomeScore = match.HomeScore,
        AwayScore = match.AwayScore,
        Status = match.Status,
        Notes = match.Notes
    };
}

