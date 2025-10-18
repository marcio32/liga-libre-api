using LigaLibre.Application.DTOs;
using LigaLibre.Domain.Entities;
using Mapster;

namespace LigaLibre.Application.Mappings;

public static class MappingConfig
{

    public static void RegisterMappings()
    {
        var config = TypeAdapterConfig.GlobalSettings;

        // Referee Mappings
        config.NewConfig<Referee, RefereeDto>();
        config.NewConfig<CreateRefereeDto, Referee>()
            .Map(dest => dest.IsActive, src => true)
            .Map(dest => dest.CreatedAt, src => DateTime.UtcNow);

        // PLayer Mappings
        config.NewConfig<Player, PlayerDto>();
        config.NewConfig<CreatePlayerDto, Player>()
            .Map(dest => dest.IsActive, src => true)
            .Map(dest => dest.CreatedAt, src => DateTime.UtcNow);

        // Club Mappings
        config.NewConfig<Club, ClubDto>();
        config.NewConfig<CreateClubDto, Club>()
            .Map(dest => dest.CreatedAt, src => DateTime.UtcNow);

        // Match Mappings
        config.NewConfig<Match, MatchDto>()
            .Map(dest => dest.HomeClubName, src => src.HomeClub != null ? src.HomeClub.Name : string.Empty)
            .Map(dest => dest.AwayClubName, src => src.AwayClub != null ? src.AwayClub.Name : string.Empty)
            .Map(dest => dest.RefereeName, src => src.Referee != null ? $"{src.Referee.FirstName} {src.Referee.LastName}" : string.Empty);
        config.NewConfig<CreateMatchDto, Match>()
            .Map(dest => dest.CreatedAt, src => DateTime.UtcNow);

        //Statistics Mappings
        config.NewConfig<Player, LeagueStatisticsDto.TopScorersDto>()
            .Map(dest => dest.PlayerName, src => $"{src.FirstName} {src.LastName}")
            .Map(dest => dest.ClubName, src => src.Club != null ? src.Club.Name : "Sin Club");

        config.NewConfig<Player, LeagueStatisticsDto.TopAssistsDto>()
            .Map(dest => dest.PlayerName, src => $"{src.FirstName} {src.LastName}")
            .Map(dest => dest.ClubName, src => src.Club != null ? src.Club.Name : "Sin Club");

        config.NewConfig<Player, LeagueStatisticsDto.PositionStatsDto>();

        config.NewConfig<Match, LeagueStatisticsDto.RecentMatchDto>()
            .Map(dest => dest.HomeClub, src => src.HomeClub != null ? src.HomeClub.Name : "TBD")
            .Map(dest => dest.AwayClub, src => src.AwayClub != null ? src.AwayClub.Name : "TBD");

        config.Compile();

    }
}

