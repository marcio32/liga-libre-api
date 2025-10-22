using LigaLibre.Application.DTOs;
using LigaLibre.Application.Interfaces;
using LigaLibre.Domain.Entities;
using LigaLibre.Domain.Interfaces;
using Mapster;

namespace LigaLibre.Application.Services;

public class RefereeService(IRefereeRepository refereeRepository, ISqsService sqsService, IRedisCacheService cacheService) : IRefereeService
{
    public async Task<IEnumerable<RefereeDto>> GetAllRefereesAsync()
    {
        var cachedReferees = await cacheService.GetAsync<IEnumerable<RefereeDto>>("referees:all");

        if (cachedReferees != null) return cachedReferees;

        var referees = await refereeRepository.GetAllAsync();
        var refereeDtos = referees.Select(MapToDto);

        await cacheService.SetAsync("referees:all", refereeDtos, TimeSpan.FromMinutes(10));

        return refereeDtos;
    }

    public async Task<IEnumerable<RefereeDto>> GetActiveRefereesAsync()
    {
        var cachedReferees = await cacheService.GetAsync<IEnumerable<RefereeDto>>("referees:active");

        if (cachedReferees != null) return cachedReferees;

        var referees = await refereeRepository.GetActivesAsync();
        var refereeDtos = referees.Select(MapToDto);

        await cacheService.SetAsync("referees:active", refereeDtos, TimeSpan.FromMinutes(10));

        return refereeDtos;
    }

    public async Task<RefereeDto?> GetRefereeByIdAsync(int id)
    {
        var cachedReferees = await cacheService.GetAsync<RefereeDto?>($"referees:{id}");

        if (cachedReferees != null) return cachedReferees;

        var referee = await refereeRepository.GetByIdAsync(id);

        if (referee != null) await cacheService.SetAsync($"referees:{id}", MapToDto(referee), TimeSpan.FromMinutes(10));

        return referee != null ? MapToDto(referee) : null;
    }
    public async Task<RefereeDto?> GetRefereeByLicenseNumberAsync(string licenseNumber)
    {
        var cachedReferees = await cacheService.GetAsync<RefereeDto?>($"referees:license:{licenseNumber}");

        if (cachedReferees != null) return cachedReferees;

        var referee = await refereeRepository.GetByLicenseNumberAsync(licenseNumber);

        if (referee != null) await cacheService.SetAsync($"referees:license:{licenseNumber}", MapToDto(referee), TimeSpan.FromMinutes(10));

        return referee != null ? MapToDto(referee) : null;
    }

    public async Task<RefereeDto> CreateRefereeAsync(CreateRefereeDto createRefereeDto)
    {
        var existingReferee = await refereeRepository.GetByLicenseNumberAsync(createRefereeDto.LicenseNumber);
        if (existingReferee != null) throw new ArgumentException("Ya existe un arbitro con esa licencia");

        var referee = createRefereeDto.Adapt<Referee>();

        var createdReferee = await refereeRepository.CreateAsync(referee);

        await sqsService.SendMessageAsync(new
        {
            EventType = "RefereeCreated",
            RefereeId = createdReferee.Id,
            LicenseNumber = createdReferee.LicenseNumber,
            FullName = $"{createdReferee.FirstName} {createdReferee.LastName}",
            Timestamp = DateTime.UtcNow
        }, QueuNames.RefereeEvent, 0);

        await cacheService.RemoveAsync("referees:all");
        await cacheService.RemoveAsync("referees:active");

        return MapToDto(createdReferee);
    }

    public async Task<RefereeDto?> UpdateRefereeAsync(UpdateRefereeDto createRefereeDto)
    {
        var existingReferee = await refereeRepository.GetByIdAsync(createRefereeDto.Id);
        if (existingReferee == null) return null;

        createRefereeDto.Adapt(existingReferee);

        var updatedReferee = await refereeRepository.UpdateAsync(existingReferee);

        if (updatedReferee == null) return null;

        await sqsService.SendMessageAsync(new
        {
            EventType = "RefereeUpdated",
            RefereeId = updatedReferee.Id,
            LicenseNumber = updatedReferee.LicenseNumber,
            FullName = $"{updatedReferee.FirstName} {updatedReferee.LastName}",
            Timestamp = DateTime.UtcNow
        }, QueuNames.RefereeEvent, 0);

        await cacheService.RemoveAsync($"referees:{createRefereeDto.Id}");
        await cacheService.RemoveAsync("referees:all");
        await cacheService.RemoveAsync("referees:active");

        return MapToDto(updatedReferee);
    }

    public async Task<bool> DeleteRefereeAsync(int id)
    {
        var existingReferee = await refereeRepository.GetByIdAsync(id);

        await sqsService.SendMessageAsync(new
        {
            EventType = "RefereeDeleted",
            RefereeId = id,
            Timestamp = DateTime.UtcNow
        }, QueuNames.RefereeEvent, 0);

        await cacheService.RemoveAsync($"referees:{id}");
        await cacheService.RemoveAsync("referees:all");
        await cacheService.RemoveAsync("referees:active");

        return await refereeRepository.DeleteAsync(id);
    }

    private static RefereeDto MapToDto(Referee referee) => referee.Adapt<RefereeDto>();
}