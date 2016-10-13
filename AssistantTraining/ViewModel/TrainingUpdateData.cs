using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssistantTraining.ViewModel
{
    public class TrainingUpdateData
    {

        public List<WorkerPerTraining> Workers { get; set; }
        public string TrainingDate { get; set; }
        public string TrainingNumber { get; set; }
    }
    public class WorkerPerTraining
    {
        public int WorkerID { get; set; }
        public int TrainingNameId { get; set; } 
        public bool Checked { get; set; }
    }
}