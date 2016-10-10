using AssistantTraining.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssistantTraining.ViewModel
{
    public class TrainingIndexData
    {

        public IEnumerable<TrainingItemIndexData> items = null;
    }

    public class TrainingItemIndexData
    {
        public int WorkId { get; set; }
        public TrainingNames Training { get; set; }
        public Instruction Instruction { get; set; }
        public Worker Worker { get; set; }

        //public IEnumerable<Training> Trainings { get; set; }
        //public IEnumerable<Instruction> Instructions { get; set; }
        //public IEnumerable<Worker> Workers { get; set; }
    }


}