using AutoMapper;
using TeamMatches.Application.DTOs;
using TeamMatches.Domain.Models;

namespace TeamMatches.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Team, TeamDto>();
            CreateMap<TeamDto, Team>();

            CreateMap<Game, GameDto>()
            .ForMember(d => d.HomeTeamName, o => o.MapFrom(s => s.HomeTeam != null ? s.HomeTeam.Name : ""))
            .ForMember(d => d.GuestTeamName, o => o.MapFrom(s => s.GuestTeam != null ? s.GuestTeam.Name : ""));
            CreateMap<GameDto, Game>();
        }
    }
}
