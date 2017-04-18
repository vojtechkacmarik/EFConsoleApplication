using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using EFConsoleApplication.Models;

namespace EFConsoleApplication
{
    [DbConfigurationType("EFConsoleApplication.PersonDbConfiguration, EFConsoleApplication")]
    public class PersonDbContext : DbContext
    {
        public PersonDbContext() : base("name=PersonDbDatabase")
        {
            Init();
        }

        public DbSet<Person> Persons { get; set; }
        public DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public override int SaveChanges()
        {
            var changedEntities = ChangeTracker.Entries();

            foreach (var changedEntity in changedEntities)
            {
                var entityWithHistory = changedEntity.Entity as EntityWithHistoryBase;
                if (entityWithHistory == null) continue;

                var actualDate = DateTime.UtcNow;

                switch (changedEntity.State)
                {
                    case EntityState.Added:
                        entityWithHistory.Created = actualDate;
                        entityWithHistory.Modified = actualDate;
                        break;

                    case EntityState.Modified:
                        entityWithHistory.Modified = actualDate;
                        break;

                    case EntityState.Deleted:
                        entityWithHistory.IsDeleted = true;
                        entityWithHistory.Deleted = actualDate;
                        break;

                    case EntityState.Detached:
                    case EntityState.Unchanged:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return base.SaveChanges();
        }

        private void Init()
        {
            Configuration.LazyLoadingEnabled = false;
            // Configuration.AutoDetectChangesEnabled = true;
            // Configuration.ValidateOnSaveEnabled = true;

            Database.Log = Console.Write;
        }
    }
}