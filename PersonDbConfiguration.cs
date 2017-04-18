using System.Data.Entity;
using System.Data.Entity.Infrastructure.DependencyResolution;
using EFConsoleApplication.Components;

namespace EFConsoleApplication
{
    public class PersonDbConfiguration : DbConfiguration
    {
        public PersonDbConfiguration()
        {
        }
    }
}