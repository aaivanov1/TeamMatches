namespace TeamMatches.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        ITeamRepository Teams { get; }

        IGameRepository Games { get; }

        Task<int> CompleteAsync(CancellationToken cancellationToken = default);

        Task DisposeAsync();
    }
}
