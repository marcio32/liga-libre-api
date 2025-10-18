using LigaLibre.Domain.Entities;

namespace LigaLibre.Domain.Interfaces
{
    public interface IPlayerRepository
    {
        Task<Player> CreateAsync(Player player);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Player>> GetAllAsync();
        Task<IEnumerable<Player>> GetByClubIdAsync(int clubId);
        Task<Player?> GetByIdAsync(int id);
        Task<Player> UpdateAsync(Player player);
    }
}