using System;

namespace EFConsoleApplication.Components
{
    public interface IDateTimeProvider
    {
        DateTime GetUtcNow();
    }
}