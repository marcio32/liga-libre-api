using Amazon.Runtime.Internal.Util;
using LigaLibre.Application.DTOs;
using LigaLibre.Application.Interfaces;
using LigaLibre.Domain.Entities;
using LigaLibre.Domain.Interfaces;

namespace LigaLibre.Application.Services;

public class ClubService(IClubRepository clubRepository, ISqsService sqsService, IRedisCacheService cacheService) : IClubService
{

    public async Task<IEnumerable<ClubDto>> GetAllClubsAsync()
    {
        var cachedClubs = await cacheService.GetAsync<IEnumerable<ClubDto>>("clubs:all");

        if(cachedClubs != null)
        {
            return cachedClubs;
        }

        var clubs = await clubRepository.GetAllAsync();

        await cacheService.SetAsync("clubs:all", clubs);
        return clubs.Select(MapToDto);
    }
    public async Task<ClubDto?> GetClubByIdAsync(int id)
    {
        var club = await clubRepository.GetByIdAsync(id);
        return club != null ? MapToDto(club) : null;
    }
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

        await sqsService.SendMessageAsync(club, QueuNames.ClubEvent, 0);

        var createClub = await clubRepository.CreateAsync(club);
        return MapToDto(createClub);
    }
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
        return MapToDto(updatedClub);
    }
    public async Task<bool> DeleteClubAsync(int id)
    {
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

