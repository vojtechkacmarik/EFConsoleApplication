using System.Data.Entity.Migrations;

namespace EFConsoleApplication.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<PersonDbContext>
    {
        private const string ORIGINAL_DB_CONFIGURATION_NAMESPACE_AND_NAME =
            "EFConsoleApplication.Migrations.PersonDbMigrationsConfiguration";

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = false;

            ContextKey = ORIGINAL_DB_CONFIGURATION_NAMESPACE_AND_NAME;
        }

        protected override void Seed(PersonDbContext context)
        {
            context.Configuration.LazyLoadingEnabled = false;
        }
    }
}