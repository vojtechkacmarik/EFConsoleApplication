namespace EFConsoleApplication.Models
{
    public abstract class EntityBase : IEntity
    {
        public int Id { get; set; }

        protected void OnBeforeInsert()
        {
        }

        protected void OnBeforeDelete()
        {
        }
    }
}