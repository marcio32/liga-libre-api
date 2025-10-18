using LigaLibre.Application.DTOs;
using LigaLibre.Application.Interfaces;
using LigaLibre.Domain.Entities;
using LigaLibre.Domain.Interfaces;
using Mapster;

namespace LigaLibre.Application.Services;

public class PlayerService(IPlayerRepository playerRepository, IRedisCacheService cacheService, ISqsService sqsService) : IPlayerService
{
    /// <summary>
    /// Obtiene todos los jugadores del sistema
    /// </summary>
    /// <returns>Colección de DTOs de jugadores</returns>
    public async Task<IEnumerable<PlayerDto>> GetAllPlayers()
    {
        var cachedPlayers = await cacheService.GetAsync<IEnumerable<PlayerDto>>("players:all");

        if (cachedPlayers != null) return cachedPlayers;

        var players = await playerRepository.GetAllAsync();
        var playerDtos = players.Select(MapToDto);
        
        await cacheService.SetAsync("players:all", playerDtos, TimeSpan.FromMinutes(10));

        return playerDtos;
    }

    /// <summary>
    /// Obtiene los jugadores de un club específico
    /// </summary>
    /// <param name="clubId">Identificador del club</param>
    /// <returns>Colección de DTOs de jugadores del club</returns>
    public async Task<IEnumerable<PlayerDto>> GetPlayersByClubAsync(int clubId)
    {
        var cachedPlayers = await cacheService.GetAsync<IEnumerable<PlayerDto>>($"players:club:{clubId}");

        if (cachedPlayers != null) return cachedPlayers;

        var players = await playerRepository.GetByClubIdAsync(clubId);
        var playerDtos = players.Select(MapToDto);
        
        await cacheService.SetAsync($"players:club:{clubId}", playerDtos, TimeSpan.FromMinutes(10));

        return playerDtos;
    }

    /// <summary>
    /// Obtiene un jugador por su identificador
    /// </summary>
    /// <param name="id">Identificador único del jugador</param>
    /// <returns>DTO del jugador o null si no existe</returns>
    public async Task<PlayerDto?> GetPlayerByIdAsync(int id)
    {
        var cachedPlayer = await cacheService.GetAsync<PlayerDto>($"players:{id}");

        if (cachedPlayer != null) return cachedPlayer;

        var player = await playerRepository.GetByIdAsync(id);
        
        if (player != null) await cacheService.SetAsync($"players:{id}", MapToDto(player), TimeSpan.FromMinutes(10));
        
        return player != null ? MapToDto(player) : null;
    }

    /// <summary>
    /// Crea un nuevo jugador en el sistema
    /// </summary>
    /// <param name="playerDto">Datos del jugador a crear</param>
    /// <returns>DTO del jugador creado</returns>
    public async Task<PlayerDto> CreatePlayerAsync(CreatePlayerDto playerDto)
    {
        var player = playerDto.Adapt<Player>();

        var createdPlayer = await playerRepository.CreateAsync(player);

        await sqsService.SendMessageAsync(new
        {
            EventType = "PlayerCreated",
            PlayerId = createdPlayer.Id,
            FirstName = createdPlayer.FirstName,
            LastName = createdPlayer.LastName,
            Position = createdPlayer.Position,
            ClubId = createdPlayer.ClubId,
            Timestamp = DateTime.UtcNow
        }, QueuNames.PlayerEvent, 0);

        await cacheService.RemoveAsync("players:all");
        await cacheService.RemoveAsync($"players:club:{createdPlayer.ClubId}");
        await cacheService.RemoveAsync("statistics:league");
        await cacheService.RemoveAsync("statistics:players");

        return MapToDto(createdPlayer);
    }

    /// <summary>
    /// Actualiza la información de un jugador existente
    /// </summary>
    /// <param name="id">Identificador del jugador a actualizar</param>
    /// <param name="playerDto">Datos actualizados del jugador</param>
    /// <returns>DTO del jugador actualizado</returns>
    public async Task<PlayerDto> UpdatePlayerAsync(int id, UpdatePlayerDto playerDto)
    {
        var existingPlayer = await playerRepository.GetByIdAsync(id);

        if (existingPlayer == null)
        {
            return new PlayerDto();
        }

        var oldClubId = existingPlayer.ClubId;

        playerDto.Adapt(existingPlayer);

        var updatedPlayer = await playerRepository.UpdateAsync(existingPlayer);

        await sqsService.SendMessageAsync(new
        {
            EventType = "PlayerUpdated",
            PlayerId = updatedPlayer.Id,
            FirstName = updatedPlayer.FirstName,
            LastName = updatedPlayer.LastName,
            Position = updatedPlayer.Position,
            ClubId = updatedPlayer.ClubId,
            Timestamp = DateTime.UtcNow
        }, QueuNames.PlayerEvent, 0);

        await cacheService.RemoveAsync("players:all");
        await cacheService.RemoveAsync($"players:{updatedPlayer.Id}");

        return MapToDto(updatedPlayer);
    }

    /// <summary>
    /// Elimina un jugador del sistema
    /// </summary>
    /// <param name="id">Identificador del jugador a eliminar</param>
    /// <returns>True si se eliminó correctamente, false en caso contrario</returns>
    public async Task<bool> DeletePlayerAsync(int id)
    {
        var existingPlayer = await playerRepository.GetByIdAsync(id);

        await sqsService.SendMessageAsync(new
        {
            EventType = "PlayerDeleted",
            PlayerId = id,
            Timestamp = DateTime.UtcNow
        }, QueuNames.PlayerEvent, 0);

        await cacheService.RemoveAsync("players:all");
        await cacheService.RemoveAsync($"players:{id}");

        return await playerRepository.DeleteAsync(id);
    }

    private static PlayerDto MapToDto(Player player) => player.Adapt<PlayerDto>();

}