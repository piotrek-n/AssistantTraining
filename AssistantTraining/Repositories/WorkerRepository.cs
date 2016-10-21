﻿using AssistantTraining.DAL;
using AssistantTraining.Models;
using AssistantTraining.ViewModel;
using System;
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
            var newInstructions =
            (from i in db.Instructions
             group i by i.Number into groupedI
             let maxVersion = groupedI.Max(gt => gt.Version)
             select new InstructionLatestVersion
             {
                 Key = groupedI.Key,
                 maxVersion = maxVersion,
                 ID = groupedI.FirstOrDefault(gt2 => gt2.Version == maxVersion).ID
             }).ToList();

            var trainings = db.TrainingGroups.Include("Instruction").Include("TrainingNames")
                              .ToList().Where(x => newInstructions.Any(ni => ni.ID == x.InstructionId)).AsQueryable<TrainingGroup>();

            return trainings;
        }

        public IQueryable<TrainingWorkersGridData> GetWorkersByTraining(string term, string type)
        {
            IQueryable<TrainingWorkersGridData> lst = new List<TrainingWorkersGridData>().AsQueryable();
            int itemID;
            bool res = int.TryParse(term, out itemID);
            DateTime dateForNull  = new DateTime(1900, 1, 1);

            if (!string.IsNullOrEmpty(type) && type.Equals("trained") && res)
            {
                lst = (from t in db.Trainings
                       where
                         t.TrainingName.ID == itemID && t.DateOfTraining > dateForNull
                       select new TrainingWorkersGridData
                       {
                           WorkerLastName = t.Worker.LastName,
                           WorkerFirstMidName = t.Worker.FirstMidName,
                           DateOfTraining = t.DateOfTraining,
                           WorkerFullName = t.Worker.LastName + "  " + t.Worker.FirstMidName
                       }).Distinct();

                return lst;
            }
            else if (!string.IsNullOrEmpty(type) && type.Equals("untrained") && res)
            {
                lst = (from t in db.Trainings
                       where
                         t.DateOfTraining == new DateTime(1900, 1, 1)
                       select new TrainingWorkersGridData
                       {
                           //WorkerLastName = t.Worker.LastName,
                           //WorkerFirstMidName = t.Worker.FirstMidName,
                           WorkerID = t.Worker.ID,
                           TrainingNameId = t.TrainingNameId,
                           //DateOfTraining = t.DateOfTraining,
                           //TrainingNumber = t.TrainingName.Number,
                           //InstructionNumber = t.Instruction.Number,
                           WorkerFullName = t.Worker.LastName + "  " + t.Worker.FirstMidName
                       }).Distinct();

                return lst;
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