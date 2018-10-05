namespace AngularJSAuthentication.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class relationship103 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "Merchant_Id", c => c.Int());
            CreateIndex("dbo.Employees", "Merchant_Id");
            AddForeignKey("dbo.Employees", "Merchant_Id", "dbo.Merchants", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Employees", "Merchant_Id", "dbo.Merchants");
            DropIndex("dbo.Employees", new[] { "Merchant_Id" });
            DropColumn("dbo.Employees", "Merchant_Id");
        }
    }
}
