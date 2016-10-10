namespace AssistantTraining.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIcollectionTrainingGruop : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Instructions", "TrainingGroup_ID", c => c.Int());
            AddColumn("dbo.TrainingNames1", "TrainingGroup_ID", c => c.Int());
            CreateIndex("dbo.Instructions", "TrainingGroup_ID");
            CreateIndex("dbo.TrainingNames1", "TrainingGroup_ID");
            AddForeignKey("dbo.Instructions", "TrainingGroup_ID", "dbo.TrainingGroups", "ID");
            AddForeignKey("dbo.TrainingNames1", "TrainingGroup_ID", "dbo.TrainingGroups", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TrainingNames1", "TrainingGroup_ID", "dbo.TrainingGroups");
            DropForeignKey("dbo.Instructions", "TrainingGroup_ID", "dbo.TrainingGroups");
            DropIndex("dbo.TrainingNames1", new[] { "TrainingGroup_ID" });
            DropIndex("dbo.Instructions", new[] { "TrainingGroup_ID" });
            DropColumn("dbo.TrainingNames1", "TrainingGroup_ID");
            DropColumn("dbo.Instructions", "TrainingGroup_ID");
        }
    }
}
