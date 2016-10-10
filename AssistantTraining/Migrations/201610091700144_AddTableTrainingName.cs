namespace AssistantTraining.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTableTrainingName : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TrainingNames",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Number = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.Trainings", "TrainingId", c => c.Int(nullable: false));
            AddColumn("dbo.Trainings", "TrainingName_ID", c => c.Int());
            CreateIndex("dbo.Trainings", "TrainingName_ID");
            AddForeignKey("dbo.Trainings", "TrainingName_ID", "dbo.TrainingNames", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Trainings", "TrainingName_ID", "dbo.TrainingNames");
            DropIndex("dbo.Trainings", new[] { "TrainingName_ID" });
            DropColumn("dbo.Trainings", "TrainingName_ID");
            DropColumn("dbo.Trainings", "TrainingId");
            DropTable("dbo.TrainingNames");
        }
    }
}
