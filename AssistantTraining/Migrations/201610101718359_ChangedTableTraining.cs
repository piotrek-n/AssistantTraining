namespace AssistantTraining.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedTableTraining : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.TrainingNames", newName: "__mig_tmp__0");
            RenameTable(name: "dbo.Trainings", newName: "TrainingNames");
            RenameTable(name: "__mig_tmp__0", newName: "TrainingNames1");
        }
        
        public override void Down()
        {
            RenameTable(name: "TrainingNames1", newName: "__mig_tmp__0");
            RenameTable(name: "dbo.TrainingNames", newName: "Trainings");
            RenameTable(name: "dbo.__mig_tmp__0", newName: "TrainingNames");
        }
    }
}
