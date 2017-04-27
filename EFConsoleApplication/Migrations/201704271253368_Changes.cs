namespace EFConsoleApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Changes : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Address", "PersonId", "dbo.Person");
            AddForeignKey("dbo.Address", "PersonId", "dbo.Person", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Address", "PersonId", "dbo.Person");
            AddForeignKey("dbo.Address", "PersonId", "dbo.Person", "Id", cascadeDelete: true);
        }
    }
}
