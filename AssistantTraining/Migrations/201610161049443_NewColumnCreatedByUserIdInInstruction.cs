namespace AssistantTraining.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewColumnCreatedByUserIdInInstruction : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Instructions", "CreatedByUserId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Instructions", "CreatedByUserId");
        }
    }
}
