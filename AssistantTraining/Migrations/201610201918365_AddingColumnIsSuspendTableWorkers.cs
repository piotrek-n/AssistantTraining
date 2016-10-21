namespace AssistantTraining.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingColumnIsSuspendTableWorkers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Workers", "IsSuspend", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Workers", "IsSuspend");
        }
    }
}
