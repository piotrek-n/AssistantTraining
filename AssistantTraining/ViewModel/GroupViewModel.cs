using AssistantTraining.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssistantTraining.ViewModel
{
    public class GroupViewModel
    {
        public int ID { get; set; }

        public string GroupName { get; set; }

        public DateTime TimeOfCreation { get; set; }

        public DateTime TimeOfModification { get; set; }

        public string Tag { get; set; }

        public virtual ICollection<Instruction> Instructions { get; set; }

        public int RowNo { get; set; }
    }
}