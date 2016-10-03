using AssistantTraining.DAL;
using AssistantTraining.Models;
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
    }
}