using LigaLibre.Domain.Entities;

namespace LigaLibre.Domain.Interfaces
{
    public interface IClubRepository
    {
        Task<IEnumerable<Club>> GetAllAsync();
        Task<Club?> GetByIdAsync(int id);
        Task<Club> CreateAsync(Club club);
        Task<Club> UpdateAsync(Club club);
        Task<bool> DeleteAsync(int id);
    }
}
