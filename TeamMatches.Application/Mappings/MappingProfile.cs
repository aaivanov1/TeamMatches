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

            CreateMap<Game, GameDto>();
            CreateMap<GameDto, Game>();
        }
    }
}
