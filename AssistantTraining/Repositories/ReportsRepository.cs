using AssistantTraining.DAL;
using Newtonsoft.Json;
using System.Linq;

namespace AssistantTraining.Repositories
{
    public class ReportsRepository
    {
        private static AssistantTrainingContext db = new AssistantTrainingContext();

        public ReportsRepository()
        {
        }

        /// <summary>
        /// „Instrukcje” (lista instrukcji, do których nie zostały utworzone szkolenia)
        /// </summary>
        public static string InstructionsWithoutTraining()
        {


            var result = (from max_ins in (
                (from i in db.Instructions
                 join ig in (
                     (from Instructions in db.Instructions
                      group Instructions by new
                      {
                          Instructions.Number
                      } into g
                      select new
                      {
                          Ver = g.Max(p => p.Version),
                          g.Key.Number
                      }))
                       on new { i.Version, i.Number }
                   equals new { Version = ig.Ver, ig.Number }
                 select new
                 {
                     i.ID,
                     i.Version,
                     i.Number
                 }))
                          join tg in db.TrainingGroups on new { InstructionId = max_ins.ID } equals new { InstructionId = tg.InstructionId } into tg_join
                          from tg in tg_join.DefaultIfEmpty()
                          where
                            tg.InstructionId == null
                          select new
                          {
                              max_ins.Number,
                              max_ins.Version
                          }
             ).ToList().Select((currRow, index) => new { Number = currRow.Number, Version = currRow.Version, DT_RowId = index + 1 });

            var json2 = JsonConvert.SerializeObject(new
            {
                data = result
            });

            return json2.Insert(1, @"columns: [
                                    {
                                                    title: ""Number"", data: ""Number""
                                                        },
                                    {
                                                    title: ""Version"", data: ""Version""
                                    },
                                    {
                                                    data: null,
                                                    className: ""center"",
                                                    defaultContent: '<a href="""" class=""editor_edit"">Edit</a>'
                                    }],"
                                );
        }

        public static string WorkersWithoutTraining()
        {
            string json="";
            var result = (from instr in db.Instructions
                          join in_worker in (
                              (from worker_inst in (
                                  (from ig in db.InstructionGroups
                                   join gw in db.GroupWorkers on new { GroupId = ig.GroupId } equals new { GroupId = (int?)gw.GroupId }
                                   join new_inst in (
             (from max_ins in (
                 (from i in db.Instructions
                                           join ig in (
                     (from Instructions in db.Instructions
                                               group Instructions by new
                                               {
                                                   Instructions.Number
                                               } into g
                                               select new
                                               {
                                                   Ver = g.Max(p => p.Version),
                                                   g.Key.Number
                                               }))
                       on new { i.Version, i.Number }
                   equals new { Version = ig.Ver, ig.Number }
                                           select new
                                           {
                                               i.ID,
                                               i.Version,
                                               i.Number
                                           }))
                                       join tg in db.TrainingGroups on new { InstructionId = max_ins.ID } equals new { InstructionId = tg.InstructionId } into tg_join
                                       from tg in tg_join.DefaultIfEmpty()
                                       where
               tg.InstructionId != null
                                       select new
                                       {
                                           max_ins.ID,
                                           max_ins.Number
                                       })) on new { ID = ig.InstructionId } equals new { ID = (int?)new_inst.ID }
                                   select new
                                   {
                                       FirstMidName = gw.Worker.FirstMidName,
                                       LastName = gw.Worker.LastName,
                                       WorkerID = gw.Worker.ID,
                                       InstructionID = ig.ID
                                   }))
                               join train in db.Trainings on new { WorkerID = worker_inst.WorkerID } equals new { WorkerID = train.WorkerId } into train_join
                               from train in train_join.DefaultIfEmpty()
                               select new
                               {
                                   WorkerIDD = worker_inst.WorkerID,
                                   worker_inst.FirstMidName,
                                   worker_inst.LastName,
                                   InstructionIDD = worker_inst.InstructionID
                               })) on new { InstructionIDD = instr.ID } equals new { InstructionIDD = in_worker.InstructionIDD }
                          select new
                          {
                              instr.Number,
                              instr.Version,
                              in_worker.WorkerIDD,
                              in_worker.LastName,
                              in_worker.FirstMidName
                          });
            return json;
        }
    }
}