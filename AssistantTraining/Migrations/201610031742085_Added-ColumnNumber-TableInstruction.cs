namespace AssistantTraining.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedColumnNumberTableInstruction : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Instructions", "Number", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Instructions", "Number");
        }
    }
}
