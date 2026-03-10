using Microsoft.AspNetCore.Mvc;
using TeamMatches.Application.DTOs;
using TeamMatches.Application.Interfaces;

namespace TeamMatches.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaderboardController : Controller
    {
        private readonly ILeaderboardService _leaderboardService;

        public LeaderboardController(ILeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IList<RankDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IList<RankDto>>> Get()
        {
            var result = await _leaderboardService.GetRankingsAsync();
            return Ok(result);
        }
    }
}
