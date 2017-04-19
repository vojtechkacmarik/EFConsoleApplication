using System;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using EFConsoleApplication.Components;

namespace EFConsoleApplication
{
    public class CreatedAndModifiedDateInterceptor : IDbCommandTreeInterceptor
    {
        private readonly IDateTimeProvider m_DateTimeProvider;

        public CreatedAndModifiedDateInterceptor(IDateTimeProvider dateTimeProvider)
        {
            if (dateTimeProvider == null) throw new ArgumentNullException(nameof(dateTimeProvider));

            m_DateTimeProvider = dateTimeProvider;
        }

        public void TreeCreated(DbCommandTreeInterceptionContext interceptionContext)
        {
            if (interceptionContext.OriginalResult.DataSpace != DataSpace.SSpace)
            {
                return;
            }

            var insertCommand = interceptionContext.Result as DbInsertCommandTree;
            if (insertCommand != null)
            {
                interceptionContext.Result = HandleInsertCommand(insertCommand);
            }

            var updateCommand = interceptionContext.OriginalResult as DbUpdateCommandTree;
            if (updateCommand != null)
            {
                interceptionContext.Result = HandleUpdateCommand(updateCommand);
            }
        }

        private DbCommandTree HandleInsertCommand(DbInsertCommandTree insertCommand)
        {
            var now = m_DateTimeProvider.GetUtcNow();

            var setClauses = insertCommand.SetClauses
                .Select(clause => clause.UpdateIfMatch(Constants.CREATED_COLUMN_NAME, DbExpression.FromDateTime(now)))
                .Select(clause => clause.UpdateIfMatch(Constants.MODIFIED_COLUMN_NAME, DbExpression.FromDateTime(now)))
                .ToList();

            return new DbInsertCommandTree(
                insertCommand.MetadataWorkspace,
                insertCommand.DataSpace,
                insertCommand.Target,
                setClauses.AsReadOnly(),
                insertCommand.Returning);
        }

        private DbCommandTree HandleUpdateCommand(DbUpdateCommandTree updateCommand)
        {
            var now = m_DateTimeProvider.GetUtcNow();

            var setClauses = updateCommand.SetClauses
                .Select(clause => clause.UpdateIfMatch(Constants.MODIFIED_COLUMN_NAME, DbExpression.FromDateTime(now)))
                .ToList();

            return new DbUpdateCommandTree(
                updateCommand.MetadataWorkspace,
                updateCommand.DataSpace,
                updateCommand.Target,
                updateCommand.Predicate,
                setClauses.AsReadOnly(), null);
        }
    }
}