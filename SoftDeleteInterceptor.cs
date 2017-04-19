using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using EFConsoleApplication.Components;

namespace EFConsoleApplication
{
    public class SoftDeleteInterceptor : IDbCommandTreeInterceptor
    {
        private readonly IDateTimeProvider m_DateTimeProvider;

        public SoftDeleteInterceptor(IDateTimeProvider dateTimeProvider)
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

            var queryCommand = interceptionContext.Result as DbQueryCommandTree;
            if (queryCommand != null)
            {
                interceptionContext.Result = HandleQueryCommand(queryCommand);
            }

            var deleteCommand = interceptionContext.OriginalResult as DbDeleteCommandTree;
            if (deleteCommand != null)
            {
                interceptionContext.Result = HandleDeleteCommand(deleteCommand);
            }
        }

        private static DbCommandTree HandleQueryCommand(DbQueryCommandTree queryCommand)
        {
            var newQuery = queryCommand.Query.Accept(new SoftDeleteQueryVisitor());
            return new DbQueryCommandTree(
                queryCommand.MetadataWorkspace,
                queryCommand.DataSpace,
                newQuery);
        }

        private DbCommandTree HandleDeleteCommand(DbDeleteCommandTree deleteCommand)
        {
            var setClauses = new List<DbModificationClause>();
            var table = (EntityType)deleteCommand.Target.VariableType.EdmType;

            if (table.Properties.All(p => p.Name != Constants.IS_DELETED_COLUMN_NAME))
            {
                return deleteCommand;
            }

            var now = m_DateTimeProvider.GetUtcNow();

            setClauses.Add(DbExpressionBuilder.SetClause(
                deleteCommand.Target.VariableType.Variable(deleteCommand.Target.VariableName).Property(Constants.IS_DELETED_COLUMN_NAME),
                DbExpression.FromBoolean(true)));
            setClauses.Add(DbExpressionBuilder.SetClause(
                deleteCommand.Target.VariableType.Variable(deleteCommand.Target.VariableName).Property(Constants.DELETED_COLUMN_NAME),
                DbExpression.FromDateTime(now)));

            return new DbUpdateCommandTree(
                deleteCommand.MetadataWorkspace,
                deleteCommand.DataSpace,
                deleteCommand.Target,
                deleteCommand.Predicate,
                setClauses.AsReadOnly(), null);
        }
    }
}