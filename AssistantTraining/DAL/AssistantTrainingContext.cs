using AssistantTraining.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace AssistantTraining.DAL
{
    public class AssistantTrainingContext : DbContext
    {
        public AssistantTrainingContext() : base("name=AssistantTrainingModel")
        {
        }

        public DbSet<Worker> Workers { get; set; }
        public DbSet<Instruction> Instructions { get; set; }
        public DbSet<GroupInstruction> GroupInstructions { get; set; }

        public System.Data.Entity.DbSet<AssistantTraining.Models.Group> Groups { get; set; }
    }
}