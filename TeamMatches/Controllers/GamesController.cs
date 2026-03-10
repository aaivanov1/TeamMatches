using Microsoft.AspNetCore.Mvc;
using TeamMatches.Api.Models;
using TeamMatches.Application.DTOs;
using TeamMatches.Application.Interfaces;

namespace TeamMatches.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : Controller
    {
        private readonly IGameService _gameService;

        public GamesController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IList<GameDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IList<GameDto>>> GetAll()
        {
            var result = await _gameService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(GameDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<GameDto>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _gameService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(IList<GameDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<GameDto>> Create(CreateGameModel request, CancellationToken cancellationToken)
        {
            var result = await _gameService.CreateAsync(
                request.HomeTeamId,
                request.GuestTeamId,
                request.HomeScore,
                request.GuestScore,
                request.PlayedOnUtc);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(GameDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<GameDto>> Update(Guid id, UpdateGameModel request, CancellationToken cancellationToken)
        {
            var result = await _gameService.UpdateAsync(
                id,
                request.HomeTeamId,
                request.GuestTeamId,
                request.HomeScore,
                request.GuestScore,
                request.PlayedOnUtc);

            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _gameService.DeleteAsync(id);
            return NoContent();
        }

    }
}
