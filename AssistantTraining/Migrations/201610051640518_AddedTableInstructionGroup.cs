namespace AssistantTraining.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTableInstructionGroup : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InstructionGroups",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TimeOfCreation = c.DateTime(nullable: false),
                        TimeOfModification = c.DateTime(nullable: false),
                        GroupId = c.Int(nullable: false),
                        InstructionId = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: true)
                .ForeignKey("dbo.Instructions", t => t.InstructionId)
                .Index(t => t.GroupId)
                .Index(t => t.InstructionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InstructionGroups", "InstructionId", "dbo.Instructions");
            DropForeignKey("dbo.InstructionGroups", "GroupId", "dbo.Groups");
            DropIndex("dbo.InstructionGroups", new[] { "InstructionId" });
            DropIndex("dbo.InstructionGroups", new[] { "GroupId" });
            DropTable("dbo.InstructionGroups");
        }
    }
}
