using System;

namespace EFConsoleApplication.Models
{
    [SoftDelete("IsDeleted")]
    public abstract class EntityWithHistoryBase : EntityBase, IChangeTracking, ISoftDelete
    {
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public DateTime? Deleted { get; set; }
        public bool IsDeleted { get; set; }
    }
}