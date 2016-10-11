using AssistantTraining.DAL;
using AssistantTraining.Models;
using AssistantTraining.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace AssistantTraining.Repositories
{
    public class WorkerRepository
    {
        private AssistantTrainingContext db = new AssistantTrainingContext();

        public WorkerRepository()
        {
        }

        public List<Group> GetAllGroups()
        {
            List<Group> groups = db.Groups.OrderBy(x => x.GroupName).ToList();
            return groups;
        }

        public List<Group> GetGroupsById(List<int> ids)
        {
            List<Group> groups = db.Groups.Where(g => ids.Contains(g.ID)).OrderBy(x => x.GroupName).ToList();
            return groups;
        }

        public IQueryable<TrainingGroup> GetTrainings()
        {
            var trainings = db.TrainingGroups.Include("Instruction").Include("TrainingNames").AsQueryable<TrainingGroup>();
            return trainings;
        }

        public IQueryable<TrainingWorkersGridData> GetWorkersByTraining(string term, string type)
        {
            IQueryable<TrainingWorkersGridData> lst = new List<TrainingWorkersGridData>().AsQueryable(); 
            int itemID;
            bool res = int.TryParse(term, out itemID);

            if (!string.IsNullOrEmpty(type) && type.Equals("trained") && res)
            {
                lst = (from t in db.Trainings
                            //where
                            //  t.TrainingName.ID == itemID
                            select new TrainingWorkersGridData
                            {
                                WorkerLastName = t.Worker.LastName,
                                WorkerFirstMidName = t.Worker.FirstMidName,
                                DateOfTraining = t.DateOfTraining
                            });

                return lst;
            }
            else if (!string.IsNullOrEmpty(type) &&  type.Equals("untrained") && res)
            {

            }
            return lst;
        }

        public IQueryable<TrainingWorkersGridData> GetWorkersByTraining()
        {
            IQueryable<TrainingWorkersGridData> lst = new List<TrainingWorkersGridData>().AsQueryable();

            lst = (from t in db.Trainings
                       //where
                       //  t.TrainingName.ID == 1
                       select new TrainingWorkersGridData
                       {
                           WorkerLastName = t.Worker.LastName,
                           WorkerFirstMidName = t.Worker.FirstMidName,
                           DateOfTraining = t.DateOfTraining
                       });

                return lst;
   
        }
    }
}