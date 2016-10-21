using AssistantTraining.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace AssistantTraining.DAL
{
    //https://www.simple-talk.com/dotnet/net-framework/deploying-an-entity-framework-database-into-production/
    //http://www.thereformedprogrammer.net/handling-entity-framework-database-migrations-in-production-part-4-release-of-efschemacompare/
    //http://www.radicalgeek.co.uk/Post/11/using-an-entity-framework-code-first-database-from-development-to-production
    //https://www.stevefenton.co.uk/category/automation/
    public class AssistantTrainingContext : IdentityDbContext<ApplicationUser>
    {
        public AssistantTrainingContext() : base("name=AssistantTrainingModel")
        {
        }

        public DbSet<Worker> Workers { get; set; }
        public DbSet<Instruction> Instructions { get; set; }
        public DbSet<GroupWorker> GroupWorkers { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Training> Trainings { get; set; }
        public DbSet<TrainingName> TrainingNames { get; set; }
        public DbSet<InstructionGroup> InstructionGroups { get; set; }
        public DbSet<TrainingGroup> TrainingGroups { get; set; }

        public static AssistantTrainingContext Create()
        {
            return new AssistantTrainingContext();
        }
    }
}