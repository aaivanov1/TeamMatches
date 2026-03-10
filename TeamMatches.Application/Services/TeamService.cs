using AutoMapper;
using TeamMatches.Application.DTOs;
using TeamMatches.Application.Interfaces;
using TeamMatches.Domain.Interfaces;

namespace TeamMatches.Application.Services
{
    public class TeamService : ITeamService
    {
        private readonly IMapper _mapper;
        private readonly ITeamRepository _teamRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TeamService(IMapper mapper,
            ITeamRepository teamRepository,
            IUnitOfWork unitOfWork)
        {
            _teamRepository = teamRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<TeamDto> CreateAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new Exception("Team name cannot be empty");

            var existing = await _teamRepository.Get
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<TeamDto>> GetAllAsync()
        {
            var teams = await _teamRepository.GetAllAsync();
            return teams.Select(_mapper.Map<TeamDto>).ToList();
        }

        public async Task<TeamDto> GetByIdAsync(Guid id)
        {
            var team = await _teamRepository.GetByIdAsync(id);

            if (team is null)
                throw new Exception($"Team with id {id} not found");

            return _mapper.Map<TeamDto>(team);
        }

        public Task<TeamDto> UpdateAsync(Guid id, string name)
        {
            throw new NotImplementedException();
        }
    }
}
