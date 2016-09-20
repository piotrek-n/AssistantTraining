using System;
using System.Collections.Generic;

namespace AssistantTraining.Models
{
    public class GroupInstruction
    {
        public int ID { get; set; }
        public DateTime TimeOfCreation { get; set; }
        public DateTime TimeOfModification { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; }

        public int WorkerId { get; set; }
        public Worker Worker { get; set; }
    }
}