namespace AngularJSAuthentication.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class relationship101 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "ServiceDemanded_Id", c => c.Int());
            AddColumn("dbo.Employees", "ServicesAssigned_Id", c => c.Int());
            AddColumn("dbo.Services", "ServiceStartTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Services", "ServiceCompletionTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Services", "ServiceEndTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Services", "IsServiceReScheduable", c => c.Boolean(nullable: false));
            AddColumn("dbo.Services", "ServiceRescheduleDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Tasks", "TaskStartTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Tasks", "TaskCompletionTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Tasks", "TaskEndTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Tasks", "IsTaskReScheduable", c => c.Boolean(nullable: false));
            AddColumn("dbo.Tasks", "TaskRescheduleDate", c => c.DateTime(nullable: false));
            CreateIndex("dbo.Customers", "ServiceDemanded_Id");
            CreateIndex("dbo.Employees", "ServicesAssigned_Id");
            AddForeignKey("dbo.Customers", "ServiceDemanded_Id", "dbo.Services", "Id");
            AddForeignKey("dbo.Employees", "ServicesAssigned_Id", "dbo.Services", "Id");
            DropColumn("dbo.Merchants", "DateRegistration");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Merchants", "DateRegistration", c => c.DateTime(nullable: false));
            DropForeignKey("dbo.Employees", "ServicesAssigned_Id", "dbo.Services");
            DropForeignKey("dbo.Customers", "ServiceDemanded_Id", "dbo.Services");
            DropIndex("dbo.Employees", new[] { "ServicesAssigned_Id" });
            DropIndex("dbo.Customers", new[] { "ServiceDemanded_Id" });
            DropColumn("dbo.Tasks", "TaskRescheduleDate");
            DropColumn("dbo.Tasks", "IsTaskReScheduable");
            DropColumn("dbo.Tasks", "TaskEndTime");
            DropColumn("dbo.Tasks", "TaskCompletionTime");
            DropColumn("dbo.Tasks", "TaskStartTime");
            DropColumn("dbo.Services", "ServiceRescheduleDate");
            DropColumn("dbo.Services", "IsServiceReScheduable");
            DropColumn("dbo.Services", "ServiceEndTime");
            DropColumn("dbo.Services", "ServiceCompletionTime");
            DropColumn("dbo.Services", "ServiceStartTime");
            DropColumn("dbo.Employees", "ServicesAssigned_Id");
            DropColumn("dbo.Customers", "ServiceDemanded_Id");
        }
    }
}
