namespace AssistantTraining.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        GroupName = c.String(),
                        TimeOfCreation = c.DateTime(nullable: false),
                        TimeOfModification = c.DateTime(nullable: false),
                        Tag = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Instructions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Number = c.String(),
                        Version = c.String(),
                        TimeOfCreation = c.DateTime(nullable: false),
                        TimeOfModification = c.DateTime(nullable: false),
                        Tag = c.String(),
                        Group_ID = c.Int(),
                        TrainingGroup_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Groups", t => t.Group_ID)
                .ForeignKey("dbo.TrainingGroups", t => t.TrainingGroup_ID)
                .Index(t => t.Group_ID)
                .Index(t => t.TrainingGroup_ID);
            
            CreateTable(
                "dbo.GroupWorkers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TimeOfCreation = c.DateTime(nullable: false),
                        TimeOfModification = c.DateTime(nullable: false),
                        GroupId = c.Int(nullable: false),
                        WorkerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: true)
                .ForeignKey("dbo.Workers", t => t.WorkerId, cascadeDelete: true)
                .Index(t => t.GroupId)
                .Index(t => t.WorkerId);
            
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
            
            CreateTable(
                "dbo.InstructionGroups",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TimeOfCreation = c.DateTime(nullable: false),
                        TimeOfModification = c.DateTime(nullable: false),
                        GroupId = c.Int(),
                        InstructionId = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Groups", t => t.GroupId)
                .ForeignKey("dbo.Instructions", t => t.InstructionId)
                .Index(t => t.GroupId)
                .Index(t => t.InstructionId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
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
            
            CreateTable(
                "dbo.TrainingNames",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Number = c.String(),
                        TrainingGroup_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.TrainingGroups", t => t.TrainingGroup_ID)
                .Index(t => t.TrainingGroup_ID);
            
            CreateTable(
                "dbo.Trainings",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        InstructionId = c.Int(nullable: false),
                        TrainingNameId = c.Int(nullable: false),
                        WorkerId = c.Int(nullable: false),
                        TimeOfCreation = c.DateTime(nullable: false),
                        TimeOfModification = c.DateTime(nullable: false),
                        DateOfTraining = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Instructions", t => t.InstructionId, cascadeDelete: true)
                .ForeignKey("dbo.TrainingNames", t => t.TrainingNameId, cascadeDelete: true)
                .ForeignKey("dbo.Workers", t => t.WorkerId, cascadeDelete: true)
                .Index(t => t.InstructionId)
                .Index(t => t.TrainingNameId)
                .Index(t => t.WorkerId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Trainings", "WorkerId", "dbo.Workers");
            DropForeignKey("dbo.Trainings", "TrainingNameId", "dbo.TrainingNames");
            DropForeignKey("dbo.Trainings", "InstructionId", "dbo.Instructions");
            DropForeignKey("dbo.TrainingNames", "TrainingGroup_ID", "dbo.TrainingGroups");
            DropForeignKey("dbo.TrainingGroups", "TrainingNameId", "dbo.TrainingNames");
            DropForeignKey("dbo.Instructions", "TrainingGroup_ID", "dbo.TrainingGroups");
            DropForeignKey("dbo.TrainingGroups", "InstructionId", "dbo.Instructions");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.InstructionGroups", "InstructionId", "dbo.Instructions");
            DropForeignKey("dbo.InstructionGroups", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.GroupWorkers", "WorkerId", "dbo.Workers");
            DropForeignKey("dbo.GroupWorkers", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.Instructions", "Group_ID", "dbo.Groups");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Trainings", new[] { "WorkerId" });
            DropIndex("dbo.Trainings", new[] { "TrainingNameId" });
            DropIndex("dbo.Trainings", new[] { "InstructionId" });
            DropIndex("dbo.TrainingNames", new[] { "TrainingGroup_ID" });
            DropIndex("dbo.TrainingGroups", new[] { "TrainingNameId" });
            DropIndex("dbo.TrainingGroups", new[] { "InstructionId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.InstructionGroups", new[] { "InstructionId" });
            DropIndex("dbo.InstructionGroups", new[] { "GroupId" });
            DropIndex("dbo.GroupWorkers", new[] { "WorkerId" });
            DropIndex("dbo.GroupWorkers", new[] { "GroupId" });
            DropIndex("dbo.Instructions", new[] { "TrainingGroup_ID" });
            DropIndex("dbo.Instructions", new[] { "Group_ID" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Trainings");
            DropTable("dbo.TrainingNames");
            DropTable("dbo.TrainingGroups");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.InstructionGroups");
            DropTable("dbo.Workers");
            DropTable("dbo.GroupWorkers");
            DropTable("dbo.Instructions");
            DropTable("dbo.Groups");
        }
    }
}
