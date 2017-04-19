namespace EFConsoleApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Address",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Street = c.String(maxLength: 250),
                        Number = c.String(nullable: false, maxLength: 25),
                        District = c.String(maxLength: 250),
                        City = c.String(nullable: false, maxLength: 250),
                        PostalCode = c.String(nullable: false, maxLength: 10),
                        County = c.String(maxLength: 250),
                        Country = c.String(nullable: false, maxLength: 250),
                        AddressType = c.Int(nullable: false),
                        PersonId = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Modified = c.DateTime(nullable: false),
                        Deleted = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Person", t => t.PersonId, cascadeDelete: true)
                .Index(t => t.PersonId);
            
            CreateTable(
                "dbo.Person",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false, maxLength: 250),
                        LastName = c.String(nullable: false, maxLength: 250),
                        BirthDate = c.DateTime(),
                        Created = c.DateTime(nullable: false),
                        Modified = c.DateTime(nullable: false),
                        Deleted = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Address", "PersonId", "dbo.Person");
            DropIndex("dbo.Address", new[] { "PersonId" });
            DropTable("dbo.Person");
            DropTable("dbo.Address");
        }
    }
}
