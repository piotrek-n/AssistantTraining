namespace AssistantTraining.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeVersionFromStringToInt : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Instructions", "Version", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Instructions", "Version", c => c.String());
        }
    }
}
