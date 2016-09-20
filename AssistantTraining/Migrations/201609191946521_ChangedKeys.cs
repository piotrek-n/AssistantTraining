namespace AssistantTraining.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedKeys : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Instructions", "GroupInstructionId", "dbo.GroupInstructions");
            DropForeignKey("dbo.GroupInstructions", "Worker_ID", "dbo.Workers");
            DropIndex("dbo.GroupInstructions", new[] { "Worker_ID" });
            DropIndex("dbo.Instructions", new[] { "GroupInstructionId" });
            RenameColumn(table: "dbo.GroupInstructions", name: "Worker_ID", newName: "WorkerId");
            AddColumn("dbo.Instructions", "GroupId", c => c.Int(nullable: false));
            AlterColumn("dbo.GroupInstructions", "WorkerId", c => c.Int(nullable: false));
            CreateIndex("dbo.GroupInstructions", "WorkerId");
            CreateIndex("dbo.Instructions", "GroupId");
            AddForeignKey("dbo.Instructions", "GroupId", "dbo.Groups", "ID", cascadeDelete: true);
            AddForeignKey("dbo.GroupInstructions", "WorkerId", "dbo.Workers", "ID", cascadeDelete: true);
            DropColumn("dbo.Instructions", "GroupInstructionId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Instructions", "GroupInstructionId", c => c.Int(nullable: false));
            DropForeignKey("dbo.GroupInstructions", "WorkerId", "dbo.Workers");
            DropForeignKey("dbo.Instructions", "GroupId", "dbo.Groups");
            DropIndex("dbo.Instructions", new[] { "GroupId" });
            DropIndex("dbo.GroupInstructions", new[] { "WorkerId" });
            AlterColumn("dbo.GroupInstructions", "WorkerId", c => c.Int());
            DropColumn("dbo.Instructions", "GroupId");
            RenameColumn(table: "dbo.GroupInstructions", name: "WorkerId", newName: "Worker_ID");
            CreateIndex("dbo.Instructions", "GroupInstructionId");
            CreateIndex("dbo.GroupInstructions", "Worker_ID");
            AddForeignKey("dbo.GroupInstructions", "Worker_ID", "dbo.Workers", "ID");
            AddForeignKey("dbo.Instructions", "GroupInstructionId", "dbo.GroupInstructions", "ID", cascadeDelete: true);
        }
    }
}
