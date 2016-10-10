namespace AssistantTraining.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingTableTrainingGroup : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Trainings", "TrainingName_ID", "dbo.TrainingNames");
            DropIndex("dbo.Trainings", new[] { "TrainingName_ID" });
            RenameColumn(table: "dbo.Trainings", name: "TrainingName_ID", newName: "TrainingNameId");
            CreateTable(
                "dbo.TrainingGroups",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TimeOfCreation = c.DateTime(nullable: false),
                        TimeOfModification = c.DateTime(nullable: false),
                        InstructionId = c.Int(nullable: false),
                        TrainingNameId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Instructions", t => t.InstructionId, cascadeDelete: true)
                .ForeignKey("dbo.TrainingNames", t => t.TrainingNameId, cascadeDelete: true)
                .Index(t => t.InstructionId)
                .Index(t => t.TrainingNameId);
            
            AlterColumn("dbo.Trainings", "TrainingNameId", c => c.Int(nullable: false));
            CreateIndex("dbo.Trainings", "TrainingNameId");
            AddForeignKey("dbo.Trainings", "TrainingNameId", "dbo.TrainingNames", "ID", cascadeDelete: true);
            DropColumn("dbo.Trainings", "TrainingId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Trainings", "TrainingId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Trainings", "TrainingNameId", "dbo.TrainingNames");
            DropForeignKey("dbo.TrainingGroups", "TrainingNameId", "dbo.TrainingNames");
            DropForeignKey("dbo.TrainingGroups", "InstructionId", "dbo.Instructions");
            DropIndex("dbo.Trainings", new[] { "TrainingNameId" });
            DropIndex("dbo.TrainingGroups", new[] { "TrainingNameId" });
            DropIndex("dbo.TrainingGroups", new[] { "InstructionId" });
            AlterColumn("dbo.Trainings", "TrainingNameId", c => c.Int());
            DropTable("dbo.TrainingGroups");
            RenameColumn(table: "dbo.Trainings", name: "TrainingNameId", newName: "TrainingName_ID");
            CreateIndex("dbo.Trainings", "TrainingName_ID");
            AddForeignKey("dbo.Trainings", "TrainingName_ID", "dbo.TrainingNames", "ID");
        }
    }
}
