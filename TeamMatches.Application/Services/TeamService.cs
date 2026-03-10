using AutoMapper;
using TeamMatches.Application.DTOs;
using TeamMatches.Application.Interfaces;
using TeamMatches.Domain.Exceptions;
using TeamMatches.Domain.Interfaces;
using TeamMatches.Domain.Models;

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
            _mapper = mapper;
            _teamRepository = teamRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<TeamDto> CreateAsync(string name)
        {
            name = name.Trim();

            if (string.IsNullOrWhiteSpace(name))
                throw new ValidationException("Team name cannot be empty");

            var existing = await _teamRepository.GetTeamByNameAsync(name);
            if (existing is not null)
                throw new ConflictException("Team with this name already exists");

            var team = new Team
            {
                Name = name,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            };

            await _teamRepository.InsertAsync(team);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<TeamDto>(team);
        }

        public async Task DeleteAsync(Guid id)
        {
            var team = await _teamRepository.GetByIdAsync(id);
            if (team is null)
                throw new NotFoundException($"Team with id {id} cannot be found");

            _teamRepository.Remove(team);
            await _unitOfWork.CompleteAsync();
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
                throw new NotFoundException($"Team with id {id} not found");

            return _mapper.Map<TeamDto>(team);
        }

        public async Task<TeamDto> UpdateAsync(Guid id, string name)
        {
            name = name.Trim();

            if (string.IsNullOrWhiteSpace(name))
                throw new ValidationException("Team name cannot be empty");

            var team = await _teamRepository.GetByIdAsync(id);
            if (team is null)
                throw new NotFoundException($"Team with id {id} not found");

            var existing = await _teamRepository.GetTeamByNameAsync(name);
            if (existing is not null)
                throw new ConflictException("Team with this name already exists");

            team.Name = name;
            team.UpdatedOnUtc = DateTime.Now;

            _teamRepository.Update(team);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<TeamDto>(team);
        }
    }
}
