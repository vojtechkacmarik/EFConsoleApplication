namespace EFConsoleApplication.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class Changes2 : DbMigration
    {
        public override void Up()
        {
            AlterTableAnnotations(
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
                    },
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SoftDeleteColumnName",
                        new AnnotationValues(oldValue: null, newValue: "IsDeleted")
                    },
                });
            
            AlterTableAnnotations(
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
                    },
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SoftDeleteColumnName",
                        new AnnotationValues(oldValue: null, newValue: "IsDeleted")
                    },
                });
            
        }
        
        public override void Down()
        {
            AlterTableAnnotations(
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
                    },
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SoftDeleteColumnName",
                        new AnnotationValues(oldValue: "IsDeleted", newValue: null)
                    },
                });
            
            AlterTableAnnotations(
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
                    },
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SoftDeleteColumnName",
                        new AnnotationValues(oldValue: "IsDeleted", newValue: null)
                    },
                });
            
        }
    }
}
