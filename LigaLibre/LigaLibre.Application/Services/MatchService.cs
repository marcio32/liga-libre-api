using LigaLibre.Application.DTOs;
using LigaLibre.Application.Interfaces;
using LigaLibre.Domain.Entities;
using LigaLibre.Domain.Interfaces;
using Mapster;

namespace LigaLibre.Application.Services;
public class MatchService(IMatchRepository matchRepository, IRedisCacheService cacheService, ISqsService sqsService) : IMatchService
{
    /// <summary>
    /// Obtiene todos los partidos del sistema
    /// </summary>
    /// <returns>Colección de DTOs de partidos</returns>
    public async Task<IEnumerable<MatchDto>> GetAllMatchesAsync()
    {
        var cachedMatches = await cacheService.GetAsync<IEnumerable<MatchDto>>("matches:all");

        if(cachedMatches != null) return cachedMatches;

        var matches = await matchRepository.GetAllAsync();
        var matchDtos = matches.Select(MapToDto);

        await cacheService.SetAsync("matches:all", matchDtos, TimeSpan.FromMinutes(10));

        return matchDtos;
    }

    /// <summary>
    /// Obtiene un partido por su identificador
    /// </summary>
    /// <param name="id">Identificador único del partido</param>
    /// <returns>DTO del partido o null si no existe</returns>
    public async Task<MatchDto?> GetMatchByIdAsync(int id)
    {
        var cachedMatches = await cacheService.GetAsync<MatchDto>($"matches:{id}");

        if (cachedMatches != null) return cachedMatches;

        var match = await matchRepository.GetByIdAsync(id);
        
        if (match != null) await cacheService.SetAsync($"matches:{id}", MapToDto(match), TimeSpan.FromMinutes(10));
      
        return match != null ? MapToDto(match) : null;
    }

    /// <summary>
    /// Obtiene los partidos de un club específico
    /// </summary>
    /// <param name="clubId">Identificador del club</param>
    /// <returns>Colección de DTOs de partidos del club</returns>
    public async Task<IEnumerable<MatchDto>> GetMatchesByClubAsync(int clubId)
    {
        var cachedMatches = await cacheService.GetAsync<IEnumerable<MatchDto>>($"matches:club:{clubId}");

        if (cachedMatches != null) return cachedMatches;

        var matches = await matchRepository.GetByClubAsync(clubId);
        var matchDtos = matches.Select(MapToDto);

        await cacheService.SetAsync($"matches:club:{clubId}", matchDtos, TimeSpan.FromMinutes(10));
        
        return matchDtos;
    }

    /// <summary>
    /// Obtiene los partidos de una jornada específica
    /// </summary>
    /// <param name="round">Número de jornada</param>
    /// <returns>Colección de DTOs de partidos de la jornada</returns>
    public async Task<IEnumerable<MatchDto>> GetMatchesByRoundAsync(int round)
    {
        var cachedMatches = await cacheService.GetAsync<IEnumerable<MatchDto>>($"matches:round:{round}");
        
        if (cachedMatches != null) return cachedMatches;

        var matches = await matchRepository.GetByRoundAsync(round);
        var matchDtos = matches.Select(MapToDto);

        await cacheService.SetAsync($"matches:round:{round}", matchDtos, TimeSpan.FromMinutes(10));
        
        return matchDtos;
    }

    /// <summary>
    /// Crea un nuevo partido en el sistema
    /// </summary>
    /// <param name="matchDto">Datos del partido a crear</param>
    /// <returns>DTO del partido creado</returns>
    public async Task<MatchDto> CreateMatchAsync(CreateMatchDto matchDto)
    {
        var match = matchDto.Adapt<Match>();
        
        var createdMatch = await matchRepository.CreateAsync(match);

        await sqsService.SendMessageAsync(new
        {
            EventType = "MatchCreated",
            MatchId = createdMatch.Id,
            Round = createdMatch.Round,
            HomeClubId = createdMatch.HomeClubId,
            AwayClubId = createdMatch.AwayClubId,
            MatchDate = createdMatch.MatchDate,
            Timestamp = DateTime.UtcNow
        }, QueuNames.MatchEvent, 0);

        await cacheService.RemoveAsync("matches:all");

        return MapToDto(createdMatch);
    }

    /// <summary>
    /// Actualiza la información de un partido existente
    /// </summary>
    /// <param name="id">Identificador del partido a actualizar</param>
    /// <param name="matchDto">Datos actualizados del partido</param>
    /// <returns>DTO del partido actualizado o null si no existe</returns>
    public async Task<MatchDto?> UpdateMatchAsync(int id, UpdateMatchDto matchDto)
    {
        var existingMatch = await matchRepository.GetByIdAsync(id);
        if (existingMatch == null) return null;

        matchDto.Adapt(existingMatch);

        var updatedMatch = await matchRepository.UpdateAsync(existingMatch);

        if (updatedMatch != null)
        {
            await sqsService.SendMessageAsync(new
            {
                EventType = "MatchUpdated",
                MatchId = updatedMatch.Id,
                HomeScore = updatedMatch.HomeScore,
                AwayScore = updatedMatch.AwayScore,
                Status = updatedMatch.Status.ToString(),
                Timestamp = DateTime.UtcNow
            }, QueuNames.MatchEvent, 0);

            await cacheService.RemoveAsync("matches:all");
            await cacheService.RemoveAsync($"matches:{updatedMatch.Id}");
        }

        return updatedMatch != null ? MapToDto(updatedMatch) : null;
    }

    /// <summary>
    /// Elimina un partido del sistema
    /// </summary>
    /// <param name="id">Identificador del partido a eliminar</param>
    /// <returns>True si se eliminó correctamente, false en caso contrario</returns>
    public async Task<bool> DeleteMatchAsync(int id)
    {
        var existingMatch = await matchRepository.GetByIdAsync(id);

        await sqsService.SendMessageAsync(new
        {
            EventType = "MatchDeleted",
            MatchId = id,
            Timestamp = DateTime.UtcNow
        }, QueuNames.MatchEvent, 0);

        await cacheService.RemoveAsync("matches:all");
        await cacheService.RemoveAsync($"matches:{id}");

        return await matchRepository.DeleteAsync(id);
    }

    private static MatchDto MapToDto(Match match) => match.Adapt<MatchDto>();
}