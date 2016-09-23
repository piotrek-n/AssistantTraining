namespace AssistantTraining.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingTraining : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Trainings",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        InstructionId = c.Int(nullable: false),
                        WorkerId = c.Int(nullable: false),
                        TimeOfCreation = c.DateTime(nullable: false),
                        TimeOfModification = c.DateTime(nullable: false),
                        DateOfTraining = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Instructions", t => t.InstructionId, cascadeDelete: true)
                .ForeignKey("dbo.Workers", t => t.WorkerId, cascadeDelete: true)
                .Index(t => t.InstructionId)
                .Index(t => t.WorkerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Trainings", "WorkerId", "dbo.Workers");
            DropForeignKey("dbo.Trainings", "InstructionId", "dbo.Instructions");
            DropIndex("dbo.Trainings", new[] { "WorkerId" });
            DropIndex("dbo.Trainings", new[] { "InstructionId" });
            DropTable("dbo.Trainings");
        }
    }
}
