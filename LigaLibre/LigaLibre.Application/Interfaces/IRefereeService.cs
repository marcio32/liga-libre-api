using LigaLibre.Application.DTOs;

namespace LigaLibre.Application.Interfaces
{
    public interface IRefereeService
    {
        Task<RefereeDto> CreateRefereeAsync(CreateRefereeDto createRefereeDto);
        Task<bool> DeleteRefereeAsync(int id);
        Task<IEnumerable<RefereeDto>> GetActiveRefereesAsync();
        Task<IEnumerable<RefereeDto>> GetAllRefereesAsync();
        Task<RefereeDto?> GetRefereeByIdAsync(int id);
        Task<RefereeDto?> GetRefereeByLicenseNumberAsync(string licenseNumber);
        Task<RefereeDto?> UpdateRefereeAsync(UpdateRefereeDto createRefereeDto);
    }
}