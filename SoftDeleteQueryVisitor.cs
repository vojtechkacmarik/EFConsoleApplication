using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;

namespace EFConsoleApplication
{
    public class SoftDeleteQueryVisitor : DefaultExpressionVisitor
    {
        public override DbExpression Visit(DbScanExpression expression)
        {
            var table = (EntityType)expression.Target.ElementType;
            if (table.Properties.All(p => p.Name != Constants.IS_DELETED_COLUMN_NAME))
            {
                return base.Visit(expression);
            }

            var binding = expression.Bind();
            return binding.Filter(
                binding.VariableType
                    .Variable(binding.VariableName)
                    .Property(Constants.IS_DELETED_COLUMN_NAME)
                    .NotEqual(DbExpression.FromBoolean(true)));
        }
    }
}