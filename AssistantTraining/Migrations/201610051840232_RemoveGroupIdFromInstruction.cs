namespace AssistantTraining.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveGroupIdFromInstruction : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Instructions", "GroupId", "dbo.Groups");
            DropIndex("dbo.Instructions", new[] { "GroupId" });
            RenameColumn(table: "dbo.Instructions", name: "GroupId", newName: "Group_ID");
            AlterColumn("dbo.Instructions", "Group_ID", c => c.Int());
            CreateIndex("dbo.Instructions", "Group_ID");
            AddForeignKey("dbo.Instructions", "Group_ID", "dbo.Groups", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Instructions", "Group_ID", "dbo.Groups");
            DropIndex("dbo.Instructions", new[] { "Group_ID" });
            AlterColumn("dbo.Instructions", "Group_ID", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.Instructions", name: "Group_ID", newName: "GroupId");
            CreateIndex("dbo.Instructions", "GroupId");
            AddForeignKey("dbo.Instructions", "GroupId", "dbo.Groups", "ID", cascadeDelete: true);
        }
    }
}
