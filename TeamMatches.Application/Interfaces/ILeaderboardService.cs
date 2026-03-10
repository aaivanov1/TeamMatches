using TeamMatches.Application.DTOs;

namespace TeamMatches.Application.Interfaces
{
    public interface ILeaderboardService
    {
        Task<IList<RankDto>> GetRankingsAsync();
    }
}
