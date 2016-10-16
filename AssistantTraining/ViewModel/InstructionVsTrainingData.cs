using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssistantTraining.ViewModel
{
    public class InstructionVsTrainingData
    {
        public string WorkerLastName { get; set; }
        public string WorkerFirstMidName { get; set; }
        public string InstructionName { get; set; }
        public int? GroupId { get; set; }
        public string InstructionVersion { get; set; }
        public string InstructionNumber { get; set; }
        public DateTime? DateOfTraining { get; set; }

        public string  DateOfTran { get { return DateOfTraining != null ? DateOfTraining.Value.ToShortDateString() : String.Empty; } }

    }
}