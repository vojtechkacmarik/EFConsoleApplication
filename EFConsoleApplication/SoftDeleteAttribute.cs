using System;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;

namespace EFConsoleApplication
{
    public class SoftDeleteAttribute : Attribute
    {
        public SoftDeleteAttribute(string columnName)
        {
            ColumnName = columnName;
        }

        public string ColumnName { get; }

        public static string GetSoftDeleteColumnName(EdmType type)
        {
            var metadataName = "customannotation:SoftDeleteColumnName";
            var metadata = type.MetadataProperties.SingleOrDefault(p => p.Name.EndsWith(metadataName));
            return (string)metadata?.Value;
        }
    }
}