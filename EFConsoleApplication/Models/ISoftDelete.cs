using System;

namespace EFConsoleApplication.Models
{
    public interface ISoftDelete
    {
        DateTime? Deleted { get; set; }
        bool IsDeleted { get; set; }
    }
}