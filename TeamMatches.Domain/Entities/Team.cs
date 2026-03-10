using TeamMatches.Domain.Interfaces;

namespace TeamMatches.Domain.Models
{
    public class Team : BaseEntity, ISoftDeleteEntity
    {

        public string Name { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public DateTime UpdatedOnUtc { get; set; }

        public bool IsDeleted { get; set; }
    }
}
