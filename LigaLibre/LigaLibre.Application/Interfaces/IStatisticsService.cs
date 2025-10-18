using LigaLibre.Application.DTOs;
using LigaLibre.Domain.Entities;
using static LigaLibre.Application.DTOs.LeagueStatisticsDto;

namespace LigaLibre.Application.Interfaces
{
    public interface IStatisticsService
    {
        Task<LeagueStatisticsDto> GetLeaguesStatisticsDtoAsync();
        Task<MatchStatisticsDto> GetMatchesStatisticsDtoAsync();
        Task<PlayerStatisticsDto> GetPlayersStatisticsDtoAsync();
    }
}