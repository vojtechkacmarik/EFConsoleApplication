namespace EFConsoleApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRowVersion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Address", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.Person", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Person", "RowVersion");
            DropColumn("dbo.Address", "RowVersion");
        }
    }
}
