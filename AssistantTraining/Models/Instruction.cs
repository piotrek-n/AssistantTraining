using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace AssistantTraining.Models
{
    public class Instruction
    {
        public int ID { get; set; }
        [DisplayName("Nazwa")]
        public string Name { get; set; }
        [DisplayName("Wersja")]
        public string Version { get; set; }
        public DateTime TimeOfCreation { get; set; }
        public DateTime TimeOfModification { get; set; }
        public string Tag { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; }

        //public int GroupInstructionId { get; set; }
        //public GroupInstruction GroupInstruction { get; set; }
    }
}