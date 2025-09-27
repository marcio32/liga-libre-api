using LigaLibre.Domain.Entities;

namespace LigaLibre.Domain.Interfaces;

public interface IRefereeRepository
{
    Task<IEnumerable<Referee>> GetAllAsync();
    Task<Referee?> GetByIdAsync(int id);
    Task<Referee?> GetByLicenseNumberAsync(string licenseNumber);
    Task<IEnumerable<Referee>> GetActivesAsync();
    Task<Referee> CreateAsync(Referee referee);
    Task<Referee?> UpdateAsync(Referee referee);
    Task<bool> DeleteAsync(int id);
}