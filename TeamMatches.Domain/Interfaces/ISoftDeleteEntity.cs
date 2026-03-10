namespace TeamMatches.Domain.Interfaces
{
    public interface ISoftDeleteEntity
    {
        public bool IsDeleted { get; set; }
    }
}
