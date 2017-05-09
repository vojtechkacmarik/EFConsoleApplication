namespace EFConsoleApplication.Models
{
    public interface IEntity
    {
        int Id { get; set; }

        byte[] RowVersion { get; set; }
    }
}