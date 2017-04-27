using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using EFConsoleApplication.Models;

namespace EFConsoleApplication
{
    [DbConfigurationType("EFConsoleApplication.PersonDbConfiguration, EFConsoleApplication")]
    public class PersonDbContext : DbContext
    {
        private SoftDeleteHelper m_SoftDeleteHelper = new SoftDeleteHelper();

        public PersonDbContext()
            : base("name=PersonDbDatabase")
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.AutoDetectChangesEnabled = true; // default
            Configuration.ValidateOnSaveEnabled = true; // default

            Database.Log = Console.Write;
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Person> Persons { get; set; }

        public override int SaveChanges()
        {
            // HandleEntityWithHistoryBase();
            // HandleDeletedEntity();

            return base.SaveChanges();
        }

        private void HandleDeletedEntity()
        {
            foreach (var entry in ChangeTracker.Entries().Where(p => p.State == EntityState.Deleted))
            {
                m_SoftDeleteHelper.SoftDelete(this, entry);
            }
        }

        private void HandleEntityWithHistoryBase()
        {
            var changedEntities = ChangeTracker.Entries();
            if (changedEntities == null) return;

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
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // ConfigureIsDeleted(modelBuilder);

            modelBuilder.Entity<Address>()
                .HasRequired(a => a.Person)
                .WithMany(x => x.Addresses)
                .HasForeignKey(a => a.PersonId)
                .WillCascadeOnDelete(false);

            var convention = new AttributeToTableAnnotationConvention<SoftDeleteAttribute, string>(
                "SoftDeleteColumnName",
                (type, attributes) => attributes.Single().ColumnName);
            modelBuilder.Conventions.Add(convention);
        }

        private static void ConfigureIsDeleted(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>()
                            .Map(m => m.Requires(Constants.IS_DELETED_COLUMN_NAME).HasValue(false))
                            .Ignore(m => m.IsDeleted);
            modelBuilder.Entity<Address>()
                .Map(m => m.Requires(Constants.IS_DELETED_COLUMN_NAME).HasValue(false))
                .Ignore(m => m.IsDeleted);
        }
    }
}