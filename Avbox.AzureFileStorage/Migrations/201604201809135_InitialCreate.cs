namespace AzureFileStorage.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AzureFileInfo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AzureUri = c.String(),
                        Link = c.String(),
                        Recipient = c.String(),
                        UploadDate = c.DateTime(nullable: false),
                        FileSize = c.Double(nullable: false),
                        FileName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AzureFileInfo");
        }
    }
}
