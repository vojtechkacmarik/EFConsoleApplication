using System;

namespace EFConsoleApplication.Models
{
    public interface IChangeTracking
    {
        DateTime Created { get; set; }
        DateTime Modified { get; set; }
    }
}