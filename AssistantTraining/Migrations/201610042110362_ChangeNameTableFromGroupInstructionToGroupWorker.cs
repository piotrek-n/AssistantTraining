namespace AssistantTraining.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeNameTableFromGroupInstructionToGroupWorker : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.GroupInstructions", newName: "GroupWorkers");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.GroupWorkers", newName: "GroupInstructions");
        }
    }
}
