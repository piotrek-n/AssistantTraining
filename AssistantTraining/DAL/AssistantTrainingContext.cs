using AssistantTraining.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace AssistantTraining.DAL
{
    public class AssistantTrainingContext: IdentityDbContext<ApplicationUser>
    {
        public AssistantTrainingContext() : base("name=AssistantTrainingModel")
        {
            
        }

        public DbSet<Worker> Workers { get; set; }
        public DbSet<Instruction> Instructions { get; set; }
        public DbSet<GroupInstruction> GroupInstructions { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Training> Trainings { get; set; }

        public static AssistantTrainingContext Create()
        {
            return new AssistantTrainingContext();
        }
    }
}