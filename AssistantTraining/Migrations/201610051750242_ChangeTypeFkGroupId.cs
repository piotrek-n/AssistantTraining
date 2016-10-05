namespace AssistantTraining.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeTypeFkGroupId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.InstructionGroups", "GroupId", "dbo.Groups");
            DropIndex("dbo.InstructionGroups", new[] { "GroupId" });
            AlterColumn("dbo.InstructionGroups", "GroupId", c => c.Int());
            CreateIndex("dbo.InstructionGroups", "GroupId");
            AddForeignKey("dbo.InstructionGroups", "GroupId", "dbo.Groups", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InstructionGroups", "GroupId", "dbo.Groups");
            DropIndex("dbo.InstructionGroups", new[] { "GroupId" });
            AlterColumn("dbo.InstructionGroups", "GroupId", c => c.Int(nullable: false));
            CreateIndex("dbo.InstructionGroups", "GroupId");
            AddForeignKey("dbo.InstructionGroups", "GroupId", "dbo.Groups", "ID", cascadeDelete: true);
        }
    }
}
