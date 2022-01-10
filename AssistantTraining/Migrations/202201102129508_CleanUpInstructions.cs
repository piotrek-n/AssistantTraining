namespace AssistantTraining.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CleanUpInstructions : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Instructions", "Group_ID", "dbo.Groups");
            DropForeignKey("dbo.Instructions", "TrainingGroup_ID", "dbo.TrainingGroups");
            DropIndex("dbo.Instructions", new[] { "Group_ID" });
            DropIndex("dbo.Instructions", new[] { "TrainingGroup_ID" });
            DropColumn("dbo.Instructions", "Group_ID");
            DropColumn("dbo.Instructions", "TrainingGroup_ID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Instructions", "TrainingGroup_ID", c => c.Int());
            AddColumn("dbo.Instructions", "Group_ID", c => c.Int());
            CreateIndex("dbo.Instructions", "TrainingGroup_ID");
            CreateIndex("dbo.Instructions", "Group_ID");
            AddForeignKey("dbo.Instructions", "TrainingGroup_ID", "dbo.TrainingGroups", "ID");
            AddForeignKey("dbo.Instructions", "Group_ID", "dbo.Groups", "ID");
        }
    }
}
