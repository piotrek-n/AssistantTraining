namespace AssistantTraining.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InstructionReminder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Instructions", "Reminder", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Instructions", "Reminder");
        }
    }
}
