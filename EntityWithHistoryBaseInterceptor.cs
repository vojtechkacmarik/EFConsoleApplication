using System;
using System.Data.Entity.Infrastructure.Interception;
using EFConsoleApplication.Components;

namespace EFConsoleApplication
{
    public class EntityWithHistoryBaseInterceptor : IDbCommandTreeInterceptor
    {
        private readonly IDateTimeProvider m_DateTimeProvider;

        public EntityWithHistoryBaseInterceptor(IDateTimeProvider dateTimeProvider)
        {
            if (dateTimeProvider == null) throw new ArgumentNullException(nameof(dateTimeProvider));

            m_DateTimeProvider = dateTimeProvider;
        }

        public void TreeCreated(DbCommandTreeInterceptionContext interceptionContext)
        {
        }
    }
}