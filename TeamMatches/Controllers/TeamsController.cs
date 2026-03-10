using Microsoft.AspNetCore.Mvc;
using TeamMatches.Api.Models;
using TeamMatches.Application.DTOs;
using TeamMatches.Application.Interfaces;

namespace TeamMatches.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamsController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IList<TeamDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _teamService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(TeamDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<TeamDto>> GetById(Guid id)
        {
            var result = await _teamService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(TeamDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<TeamDto>> Create(CreateTeamModel request)
        {
            var result = await _teamService.CreateAsync(request.Name);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(TeamDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<TeamDto>> Update(Guid id, UpdateTeamModel request)
        {
            var result = await _teamService.UpdateAsync(id, request.Name);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _teamService.DeleteAsync(id);
            return NoContent();
        }
    }
}
