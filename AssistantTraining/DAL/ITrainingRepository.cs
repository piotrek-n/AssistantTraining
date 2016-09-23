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
        IEnumerable<Training> GetTrainings();
        Training GetTrainingByID(int trainingID);
        void InsertStudent(Training training);
        void DeleteStudent(int trainingID);
        void UpdateStudent(Training training);
        void Save();
    }
}
