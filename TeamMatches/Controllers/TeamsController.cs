using Microsoft.AspNetCore.Mvc;
using TeamMatches.Application.Interfaces;

namespace TeamMatches.Api.Controllers
{
    [ApiController]
    [Route("api/teams")]
    public sealed class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamsController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _teamService.GetAllAsync();
            return Ok(result);
        }

        //[HttpGet("{id:guid}")]
        //[ProducesResponseType(typeof(TeamDto), StatusCodes.Status200OK)]
        //public async Task<ActionResult<TeamDto>> GetById(Guid id, CancellationToken cancellationToken)
        //{
        //    var result = await _teamService.GetByIdAsync(id, cancellationToken);
        //    return Ok(result);
        //}

        //[HttpPost]
        //[ProducesResponseType(typeof(TeamDto), StatusCodes.Status201Created)]
        //public async Task<ActionResult<TeamDto>> Create(CreateTeamRequest request, CancellationToken cancellationToken)
        //{
        //    var result = await _teamService.CreateAsync(request.Name, cancellationToken);
        //    return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        //}

        //[HttpPut("{id:guid}")]
        //[ProducesResponseType(typeof(TeamDto), StatusCodes.Status200OK)]
        //public async Task<ActionResult<TeamDto>> Update(Guid id, UpdateTeamRequest request, CancellationToken cancellationToken)
        //{
        //    var result = await _teamService.UpdateAsync(id, request.Name, cancellationToken);
        //    return Ok(result);
        //}

        //[HttpDelete("{id:guid}")]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        //{
        //    await _teamService.DeleteAsync(id, cancellationToken);
        //    return NoContent();
        //}
    }
}
