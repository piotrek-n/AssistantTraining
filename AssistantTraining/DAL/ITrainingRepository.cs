using AssistantTraining.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssistantTraining.DAL
{
    interface ITrainingRepository
    {
        IEnumerable<TrainingNames> GetTrainings();
        TrainingNames GetTrainingByID(int trainingID);
        void InsertStudent(TrainingNames training);
        void DeleteStudent(int trainingID);
        void UpdateStudent(TrainingNames training);
        void Save();
    }
}
