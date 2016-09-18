namespace AssistantTraining.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GroupInstructions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        GroupName = c.String(),
                        TimeOfCreation = c.DateTime(nullable: false),
                        TimeOfModification = c.DateTime(nullable: false),
                        Tag = c.String(),
                        Worker_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Workers", t => t.Worker_ID)
                .Index(t => t.Worker_ID);
            
            CreateTable(
                "dbo.Instructions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Version = c.String(),
                        TimeOfCreation = c.DateTime(nullable: false),
                        TimeOfModification = c.DateTime(nullable: false),
                        Tag = c.String(),
                        GroupInstructionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.GroupInstructions", t => t.GroupInstructionId, cascadeDelete: true)
                .Index(t => t.GroupInstructionId);
            
            CreateTable(
                "dbo.Workers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        LastName = c.String(),
                        FirstMidName = c.String(),
                        TimeOfCreation = c.DateTime(nullable: false),
                        TimeOfModification = c.DateTime(nullable: false),
                        Tag = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GroupInstructions", "Worker_ID", "dbo.Workers");
            DropForeignKey("dbo.Instructions", "GroupInstructionId", "dbo.GroupInstructions");
            DropIndex("dbo.Instructions", new[] { "GroupInstructionId" });
            DropIndex("dbo.GroupInstructions", new[] { "Worker_ID" });
            DropTable("dbo.Workers");
            DropTable("dbo.Instructions");
            DropTable("dbo.GroupInstructions");
        }
    }
}
