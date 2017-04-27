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
            var columnName = SoftDeleteAttribute.GetSoftDeleteColumnName(expression.Target.ElementType);
            var entityType = (EntityType)expression.Target.ElementType;
            if (columnName == null || entityType.Properties.All(p => p.Name != columnName))
                return base.Visit(expression);

            var binding = expression.Bind();
            return binding.Filter(
                binding.VariableType
                    .Variable(binding.VariableName)
                    .Property(columnName)
                    .NotEqual(DbExpression.FromBoolean(true)));
        }
    }
}