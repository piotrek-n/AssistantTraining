using System;

namespace AssistantTraining.Models
{
    public class Instruction
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public DateTime TimeOfCreation { get; set; }
        public DateTime TimeOfModification { get; set; }
        public string Tag { get; set; }

        public int GroupInstructionId { get; set; }
        public GroupInstruction GroupInstruction { get; set; }
    }
}