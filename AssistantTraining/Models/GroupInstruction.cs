using System;
using System.Collections.Generic;

namespace AssistantTraining.Models
{
    public class GroupInstruction
    {
        public int ID { get; set; }
        public string GroupName { get; set; }
        public DateTime TimeOfCreation { get; set; }
        public DateTime TimeOfModification { get; set; }
        public string Tag { get; set; }

        public virtual ICollection<Instruction> Instructions { get; set; }
    }
}