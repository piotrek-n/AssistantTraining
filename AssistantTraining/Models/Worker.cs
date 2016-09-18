using System;
using System.Collections.Generic;

namespace AssistantTraining.Models
{
    public class Worker
    {
        public int ID { get; set; }
        public string LastName { get; set; }
        public string FirstMidName { get; set; }
        public DateTime TimeOfCreation { get; set; }
        public DateTime TimeOfModification { get; set; }
        public string Tag { get; set; }

        public virtual ICollection<GroupInstruction> GroupInstructions { get; set; }
    }
}