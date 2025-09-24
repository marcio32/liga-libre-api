using LigaLibre.Application.DTOs;

namespace LigaLibre.Application.Interfaces
{
    public interface IPlayerService
    {
        Task<PlayerDto> CreatePlayerAsync(CreatePlayerDto playerDto);
        Task<bool> DeletePlayerAsync(int id);
        Task<PlayerDto?> GetPlayerByIdAsync(int id);
        Task<IEnumerable<PlayerDto>> GetPlayersByClubAsync(int clubId);
        Task<IEnumerable<PlayerDto>> GetAllPlayers();
        Task<PlayerDto> UpdatePlayerAsync(int id, CreatePlayerDto playerDto);
    }
}