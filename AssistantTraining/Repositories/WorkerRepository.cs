using AssistantTraining.DAL;
using AssistantTraining.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssistantTraining.Repositories
{
    public class WorkerRepository
    {
        AssistantTrainingContext db = new AssistantTrainingContext();

        public WorkerRepository()
        {
        }
        public List<Group> GetAllGroups()
        {
            List<Group> groups = db.Groups.OrderBy(x=>x.GroupName).ToList();
            return groups;
        }


    }
}