namespace AngularJSAuthentication.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class relationship102 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Services", "Merchant_Id", c => c.Int());
            AddColumn("dbo.Tasks", "Service_Id", c => c.Int());
            CreateIndex("dbo.Services", "Merchant_Id");
            CreateIndex("dbo.Tasks", "Service_Id");
            AddForeignKey("dbo.Tasks", "Service_Id", "dbo.Services", "Id");
            AddForeignKey("dbo.Services", "Merchant_Id", "dbo.Merchants", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Services", "Merchant_Id", "dbo.Merchants");
            DropForeignKey("dbo.Tasks", "Service_Id", "dbo.Services");
            DropIndex("dbo.Tasks", new[] { "Service_Id" });
            DropIndex("dbo.Services", new[] { "Merchant_Id" });
            DropColumn("dbo.Tasks", "Service_Id");
            DropColumn("dbo.Services", "Merchant_Id");
        }
    }
}
