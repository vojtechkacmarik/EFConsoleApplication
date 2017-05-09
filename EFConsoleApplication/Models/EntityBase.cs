using System.ComponentModel.DataAnnotations;

namespace EFConsoleApplication.Models
{
    public abstract class EntityBase : IEntity
    {
        public int Id { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        protected void OnBeforeInsert()
        {
        }

        protected void OnBeforeDelete()
        {
        }
    }
}