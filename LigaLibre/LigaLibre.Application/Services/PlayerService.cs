using LigaLibre.Application.DTOs;
using LigaLibre.Application.Interfaces;
using LigaLibre.Domain.Entities;
using LigaLibre.Domain.Interfaces;

namespace LigaLibre.Application.Services;

public class PlayerService(IPlayerRepository playerRepository) : IPlayerService
{
    public Task<IEnumerable<PlayerDto>> GetAllPlayers()
    {
        var players = playerRepository.GetAllAsync();
        return players.ContinueWith(t => t.Result.Select(MapToDto));
    }

    public async Task<IEnumerable<PlayerDto>> GetPlayersByClubAsync(int clubId)
    {
        var players = await playerRepository.GetByClubIdAsync(clubId);
        return players.Select(MapToDto);
    }

    public async Task<PlayerDto?> GetPlayerByIdAsync(int id)
    {
        var player = await playerRepository.GetByIdAsync(id);
        return player != null ? MapToDto(player) : null;
    }

    public async Task<PlayerDto> CreatePlayerAsync(CreatePlayerDto playerDto)
    {
        var player = new Player
        {
            Age = playerDto.Age,
            JerseuNumber = playerDto.JerseyNumber,
            FirstName = playerDto.FirstName,
            LastName = playerDto.LastName,
            Position = playerDto.Position,
            Nationality = playerDto.Nationality,
            Height = playerDto.Height,
            Weight = playerDto.Weight,
            ClubId = playerDto.ClubId,
            DateOfBirth = playerDto.DateOfBirth,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow

        };

        var createdPlayer = await playerRepository.CreateAsync(player);
        return MapToDto(createdPlayer);
    }

    public async Task<PlayerDto> UpdatePlayerAsync(int id, CreatePlayerDto playerDto)
    {
        var existingPlayer = await playerRepository.GetByIdAsync(id);

        if (existingPlayer == null)
        {
            return new PlayerDto();
        }

        existingPlayer.Age = playerDto.Age;
        existingPlayer.JerseuNumber = playerDto.JerseyNumber;
        existingPlayer.FirstName = playerDto.FirstName;
        existingPlayer.LastName = playerDto.LastName;
        existingPlayer.Position = playerDto.Position;
        existingPlayer.Nationality = playerDto.Nationality;
        existingPlayer.Height = playerDto.Height;
        existingPlayer.Weight = playerDto.Weight;
        existingPlayer.ClubId = playerDto.ClubId;
        existingPlayer.DateOfBirth = playerDto.DateOfBirth;
        existingPlayer.CreatedAt = DateTime.UtcNow;
        existingPlayer.UpdatedAt = DateTime.UtcNow;


        var updatedPlayer = await playerRepository.UpdateAsync(existingPlayer);
        return MapToDto(updatedPlayer);
    }

    public async Task<bool> DeletePlayerAsync(int id)
    {
        return await playerRepository.DeleteAsync(id);
    }

    private static PlayerDto MapToDto(Player player) => new PlayerDto
    {
        Id = player.Id,
        Age = player.Age,
        JerseuNumber = player.JerseuNumber,
        FirstName = player.FirstName,
        LastName = player.LastName,
        Position = player.Position,
        Nationality = player.Nationality,
        Height = player.Height,
        Weight = player.Weight,
        Goals = player.Goals,
        Assists = player.Assists,
        YellowCards = player.YellowCards,
        RedCards = player.RedCards,
        MatchesPlayed = player.MatchesPlayed,
        ClubId = player.ClubId,
        IsActive = player.IsActive,
        DateOfBirth = player.DateOfBirth,
        JoinedClubDate = player.JoinedClubDate,
        CreatedAt = player.CreatedAt,
        UpdatedAt = player.UpdatedAt
    };

}

