using Amazon.Runtime.Internal.Util;
using LigaLibre.Application.DTOs;
using LigaLibre.Application.Interfaces;
using LigaLibre.Domain.Entities;
using LigaLibre.Domain.Interfaces;
using System.Text.RegularExpressions;
using static System.Reflection.Metadata.BlobBuilder;

namespace LigaLibre.Application.Services;

public class ClubService(IClubRepository clubRepository, ISqsService sqsService, IRedisCacheService cacheService) : IClubService
{
    /// <summary>
    /// Obtiene todos los clubes del sistema
    /// </summary>
    /// <returns>Colección de DTOs de clubes</returns>
    public async Task<IEnumerable<ClubDto>> GetAllClubsAsync()
    {
        var cachedClubs = await cacheService.GetAsync<IEnumerable<ClubDto>>("clubs:all");

        if(cachedClubs != null) return cachedClubs;

        var clubs = await clubRepository.GetAllAsync();
        var clubDtos = clubs.Select(MapToDto);

        await cacheService.SetAsync("clubs:all", clubDtos, TimeSpan.FromMinutes(10));
        
        return clubDtos;
    }

    /// <summary>
    /// Obtiene un club por su identificador
    /// </summary>
    /// <param name="id">Identificador único del club</param>
    /// <returns>DTO del club o null si no existe</returns>
    public async Task<ClubDto?> GetClubByIdAsync(int id)
    {
        var cachedMatches = await cacheService.GetAsync<ClubDto?>($"clubs:{id}");
        if (cachedMatches != null) return cachedMatches;
        var club = await clubRepository.GetByIdAsync(id);

        if (club != null) await cacheService.SetAsync($"clubs:{id}", MapToDto(club), TimeSpan.FromMinutes(10));
        
        return club != null ? MapToDto(club) : null;
    }

    /// <summary>
    /// Crea un nuevo club en el sistema
    /// </summary>
    /// <param name="createClubDto">Datos del club a crear</param>
    /// <returns>DTO del club creado</returns>
    public async Task<ClubDto> CreateClubAsync(CreateClubDto createClubDto)
    {
        var club = new Club
        {
            Name = createClubDto.Name,
            City = createClubDto.City,
            Email = createClubDto.Email,
            NumberOfPartners = createClubDto.NumberOfPartners,
            Phone = createClubDto.Phone,
            Address = createClubDto.Address,
            StadiumName = createClubDto.StadiumName,
        };

        var createClub = await clubRepository.CreateAsync(club);

        await sqsService.SendMessageAsync(new
        {
            EventType = "ClubCreated",
            ClubId = createClub.Id,
            Name = createClub.Name,
            City = createClub.City,
            Email = createClub.Email,
            NumberOfPartners = createClub.NumberOfPartners,
            Phone = createClub.Phone,
            Address = createClub.Address,
            StadiumName = createClub.StadiumName,
            Timestamp = DateTime.UtcNow
        }, QueuNames.ClubEvent,0);

        await cacheService.RemoveAsync("clubs:all");

        return MapToDto(createClub);
    }

    /// <summary>
    /// Actualiza la información de un club existente
    /// </summary>
    /// <param name="id">Identificador del club a actualizar</param>
    /// <param name="createClubDto">Datos actualizados del club</param>
    /// <returns>DTO del club actualizado</returns>
    public async Task<ClubDto> UpdateClubAsync(int id, CreateClubDto createClubDto)
    {
        var existingClub = await clubRepository.GetByIdAsync(id);
        if (existingClub == null)
            throw new ArgumentException("Club not found");


        existingClub.Name = createClubDto.Name;
        existingClub.City = createClubDto.City;
        existingClub.Email = createClubDto.Email;
        existingClub.NumberOfPartners = createClubDto.NumberOfPartners;
        existingClub.Phone = createClubDto.Phone;
        existingClub.Address = createClubDto.Address;
        existingClub.StadiumName = createClubDto.StadiumName;

        var updatedClub = await clubRepository.UpdateAsync(existingClub);

        await sqsService.SendMessageAsync(new
        {
            EventType = "ClubUpdated",
            ClubId = updatedClub.Id,
            Name = updatedClub.Name,
            City = updatedClub.City,
            Email = updatedClub.Email,
            NumberOfPartners = updatedClub.NumberOfPartners,
            Phone = updatedClub.Phone,
            Address = updatedClub.Address,
            StadiumName = updatedClub.StadiumName,
            Timestamp = DateTime.UtcNow
        }, QueuNames.ClubEvent, 0);

        await cacheService.RemoveAsync("clubs:all");
        await cacheService.RemoveAsync($"clubs:{updatedClub.Id}");

        return MapToDto(updatedClub);
    }

    /// <summary>
    /// Elimina un club del sistema
    /// </summary>
    /// <param name="id">Identificador del club a eliminar</param>
    /// <returns>True si se eliminó correctamente, false en caso contrario</returns>
    public async Task<bool> DeleteClubAsync(int id)
    {

        await sqsService.SendMessageAsync(new
        {
            EventType = "ClubDeleted",
            ClubId = id,
            Timestamp = DateTime.UtcNow

        }, QueuNames.ClubEvent, 0);

        await cacheService.RemoveAsync("clubs:all");
        await cacheService.RemoveAsync($"clubs:{id}");

        return await clubRepository.DeleteAsync(id);
    }

    private static ClubDto MapToDto(Club club)
    {
        return new ClubDto
        {
            Id = club.Id,
            Name = club.Name,
            City = club.City,
            Email = club.Email,
            NumberOfPartners = club.NumberOfPartners,
            Phone = club.Phone,
            Address = club.Address,
            StadiumName = club.StadiumName,
        };
    }

}

