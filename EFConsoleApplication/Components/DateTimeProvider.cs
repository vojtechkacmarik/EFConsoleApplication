using System;

namespace EFConsoleApplication.Components
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime GetUtcNow()
        {
            return DateTime.UtcNow;
        }
    }
}