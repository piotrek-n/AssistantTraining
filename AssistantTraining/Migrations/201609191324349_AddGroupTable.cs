namespace AssistantTraining.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddGroupTable : DbMigration
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

            AddColumn("dbo.GroupInstructions", "GroupId", c => c.Int(nullable: false));
            CreateIndex("dbo.GroupInstructions", "GroupId");
            AddForeignKey("dbo.GroupInstructions", "GroupId", "dbo.Groups", "ID", cascadeDelete: true);
            DropColumn("dbo.GroupInstructions", "GroupName");
            DropColumn("dbo.GroupInstructions", "Tag");
        }

        public override void Down()
        {
            AddColumn("dbo.GroupInstructions", "Tag", c => c.String());
            AddColumn("dbo.GroupInstructions", "GroupName", c => c.String());
            DropForeignKey("dbo.GroupInstructions", "GroupId", "dbo.Groups");
            DropIndex("dbo.GroupInstructions", new[] { "GroupId" });
            DropColumn("dbo.GroupInstructions", "GroupId");
            DropTable("dbo.Groups");
        }
    }
}