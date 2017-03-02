using AssistantTraining.DAL;
using AssistantTraining.Models;
using AssistantTraining.Repositories;
using AssistantTraining.ViewModel;
using Microsoft.AspNet.Identity;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.SqlServer;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace AssistantTraining.Controllers
{
    [Authorize]
    public class InstructionsController : Controller
    {
        private AssistantTrainingContext db = new AssistantTrainingContext();

        // GET: Instructions
        public ActionResult Index()
        {

            int temp;
            //int max = numbers.Select(n => int.TryParse(n, out temp) ? temp : 0).Max();

            var q = from t in db.Instructions.SqlQuery("SELECT ID,Name,Number, Cast(Version as INT) as Version,TimeOfCreation,TimeOfModification,Tag,CreatedByUserId from Instructions")
                    group t by t.Number
                    into g
                    select new
                    {
                        Key = g.Key,
                        maxVersion = g.Select(n => n.Version).Max()
              //(from t2 in g select ((int)(object)(t2.Version)) ).Max()
                                                                                                     //ID = g.FirstOrDefault(gt2 => gt2.Version == (from t2 in g select int.Parse(t2.Version)).Max().ToString()).ID
        };

            var newInstructions =
                                (from i in db.Instructions
                                 group i by i.Number into groupedI
                                 let maxVersion = groupedI.Max(v => v.Version)
            select new InstructionLatestVersion
                                 {
                                     Key = groupedI.Key,
                                     maxVersion = maxVersion,
                                     ID = groupedI.FirstOrDefault(gt2 => gt2.Version == maxVersion).ID
                                 }).ToList();

            var allInstructions = db.Instructions.ToList().Where(x => newInstructions.Any(ni => ni.ID == x.ID)).ToList();
            var workerRepository = new WorkerRepository();
            var groups = workerRepository.GetAllGroups();

            List<InstructionIndexData> lstInstructionGroups = new List<InstructionIndexData>();
            int RowNo=0;
            foreach (var item in allInstructions)
            {
                var instructioGroup = new InstructionIndexData();

                RowNo += 1;
                instructioGroup.RowNo = RowNo;
                instructioGroup.ID = item.ID;
                instructioGroup.Name = item.Name;
                instructioGroup.Number = item.Number;
                instructioGroup.Version = item.Version;
                instructioGroup.UserName = item.CreatedByUserId;
                if (item.CreatedByUserId != null)
                    instructioGroup.UserName = db.Users.Find(item.CreatedByUserId).UserName;
                instructioGroup.TimeOfCreation = item.TimeOfCreation.ToShortDateString();

                var lstGroupIds = (from InstructionGroups in db.InstructionGroups
                                   where
                                     InstructionGroups.GroupId != null &&
                                     InstructionGroups.InstructionId == item.ID
                                   select new 
                                   {
                                       val= (InstructionGroups.GroupId ?? 0)
                                   }).Select(x => x.val.ToString()).ToList();
                instructioGroup.SelectedIds = lstGroupIds.ToArray();

                instructioGroup.Items = groups.Select(x => new SelectListItem
                {
                    Value = x.ID.ToString(),
                    Text = x.GroupName
                });

                lstInstructionGroups.Add(instructioGroup);
            }

            return View(lstInstructionGroups);
        }

        // GET: Instructions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var instruction = db.Instructions.Find(id);

            if (instruction == null)
            {
                return HttpNotFound();
            }
            ViewData["IdInstruction"] = id;
            InstructionDetailsData instructionGroupViewModel = new InstructionDetailsData();
            var workerRepository = new WorkerRepository();
            List<int> idsGroups = new List<int>();

            idsGroups = (from InstructionGroups in db.InstructionGroups
                               where
                                 InstructionGroups.GroupId != null &&
                                 InstructionGroups.InstructionId == id
                               select new
                               {
                                   val = (InstructionGroups.GroupId ?? 0)
                               }).Select(x => x.val).ToList();

            var groups = workerRepository.GetGroupsById(idsGroups);

            instructionGroupViewModel.Name = instruction.Name;
            instructionGroupViewModel.Version = instruction.Version;
            instructionGroupViewModel.Groups = groups;
            instructionGroupViewModel.GroupWithNumbers = groups.Select((x,i)=> new GroupViewModel()
                                                                                    {
                                                                                        GroupName=x.GroupName,
                                                                                        RowNo =i+1,
                                                                                        ID =x.ID,
                                                                                        Instructions = x.Instructions,
                                                                                        Tag = x.Tag,
                                                                                        TimeOfCreation = x.TimeOfCreation,
                                                                                        TimeOfModification = x.TimeOfModification
                                                                                    });

            instructionGroupViewModel.Items = groups.Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.GroupName
            });

            var lst = (from w in db.Workers
                       join wg in db.GroupWorkers on w.ID equals wg.WorkerId
                       join gi in db.InstructionGroups on wg.GroupId equals gi.GroupId
                       join i in db.Instructions on gi.InstructionId equals i.ID
                       join t in db.Trainings
                             on new { InstructionId = i.ID, WorkerId = wg.WorkerId }
                         equals new { t.InstructionId, t.WorkerId } into t_join
                       from t in t_join.DefaultIfEmpty()
                       where i.ID == id
                       select new InstructionVsTrainingData
                       {
                           WorkerLastName = w.LastName,
                           WorkerFirstMidName = w.FirstMidName,
                           WorkerFullName = w.LastName + " " + w.FirstMidName,
                           InstructionName = i.Name,
                           GroupId = (int?)gi.GroupId,
                           InstructionVersion = i.Version,
                           InstructionNumber = i.Number,
                           DateOfTraining = (DateTime?)t.DateOfTraining,
                           TrainingName = t.TrainingName.Number
                       }).Select(x => x).AsEnumerable().Select((w, i) =>
                           new InstructionVsTrainingData
                           {
                               WorkerLastName = w.WorkerLastName,
                               WorkerFirstMidName = w.WorkerFirstMidName,
                               WorkerFullName = w.WorkerFullName,
                               InstructionName = w.InstructionName,
                               GroupId = (int?)w.GroupId,
                               InstructionVersion = w.InstructionVersion,
                               InstructionNumber = w.InstructionNumber,
                               DateOfTraining = (DateTime?)w.DateOfTraining,
                               TrainingName = w.TrainingName,
                               RowNo = i + 1
                           }).ToList();

            instructionGroupViewModel.instructionVsTrainingList = lst.ToList();
            if (instructionGroupViewModel == null)
            {
                return HttpNotFound();
            }
            return View(instructionGroupViewModel);
        }

        // GET: Instructions/Create
        public ActionResult Create()
        {
            var instructionGroup = new InstructionIndexData();
            var workerRepository = new WorkerRepository();
            var groups = workerRepository.GetAllGroups();

            instructionGroup.Items = groups.Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.GroupName
            });

            return View(instructionGroup);
        }

        // POST: Instructions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InstructionIndexData instructionGroupViewModel)
        {
            if (ModelState.IsValid)
            {
                //db.InstructionGroupViewModels.Add(instructionGroupViewModel);
                Instruction instruction = new Instruction();
                instruction.Number = instructionGroupViewModel.Number;
                instruction.Name = instructionGroupViewModel.Name;
                instruction.Version = instructionGroupViewModel.Version;
                instruction.TimeOfCreation = DateTime.Now;
                instruction.TimeOfModification = DateTime.Now;
                string currentUserId = User.Identity.GetUserId();
                instruction.CreatedByUserId = currentUserId;
                //instruction.GroupId = Int32.Parse(instructionGroupViewModel.SelectedId);
                db.Instructions.Add(instruction);
                db.SaveChanges();

                if (!String.IsNullOrEmpty(instructionGroupViewModel.NumberOfTraining))
                {
                    TrainingName tn = new TrainingName();
                    tn.Name = String.Empty;
                    tn.Number = instructionGroupViewModel.NumberOfTraining;
                    db.TrainingNames.Add(tn);
                    db.SaveChanges();

                    TrainingGroup tg = new TrainingGroup();
                    tg.TrainingNameId = tn.ID;
                    tg.InstructionId = instruction.ID;
                    tg.TimeOfCreation = DateTime.Now;
                    tg.TimeOfModification = DateTime.Now;

                    db.TrainingGroups.Add(tg);
                    db.SaveChanges();
                }

                if (instructionGroupViewModel.SelectedIds != null && instructionGroupViewModel.SelectedIds.Count() > 0)
                {
                    foreach (var item in instructionGroupViewModel.SelectedIds)
                    {
                        var groupInstructions = new InstructionGroup()
                        {
                            InstructionId = instruction.ID,
                            TimeOfCreation = DateTime.Now,
                            TimeOfModification = DateTime.Now,
                            GroupId = Int32.Parse(item)
                        };
                        db.InstructionGroups.Add(groupInstructions);
                        db.SaveChanges();
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(instructionGroupViewModel);
        }

        // GET: Instructions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var instruction = db.Instructions.Find(id);
            //var trainingGroups = db.TrainingGroups.Include("TrainingName").Where(x => x.InstructionId.Equals(id));

            if (instruction == null)
            {
                return HttpNotFound();
            }

            InstructionEditData instructionGroupViewModel = new InstructionEditData();
            var workerRepository = new WorkerRepository();
            var groups = workerRepository.GetAllGroups();

            instructionGroupViewModel.Name = instruction.Name;
            instructionGroupViewModel.Version = instruction.Version;
            instructionGroupViewModel.Number = instruction.Number;

            instructionGroupViewModel.SelectedIds = 
                        (from InstructionGroups in db.InstructionGroups
                         where
                           InstructionGroups.GroupId != null &&
                           InstructionGroups.InstructionId == id
                         select new
                         {
                             val = (InstructionGroups.GroupId ?? 0)
                         }).Select(x => x.val.ToString()).ToList().ToArray();

            instructionGroupViewModel.Items = groups.Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.GroupName
            });

            return View(instructionGroupViewModel);
        }

        // POST: Instructions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Number,Version,SelectedIds")]InstructionEditData instructionGroupViewModel)
        {
            if (ModelState.IsValid)
            {
                Instruction instruction = new Instruction();
                instruction.ID = instructionGroupViewModel.ID;
                instruction.TimeOfModification = DateTime.Now;
                instruction.Name = instructionGroupViewModel.Name;
                instruction.Version = instructionGroupViewModel.Version;
                //instruction.GroupId = Int32.Parse(instructionGroupViewModel.SelectedId);

                db.Instructions.Attach(instruction);
                db.Entry(instruction).Property(X => X.Name).IsModified = true;
                db.Entry(instruction).Property(X => X.Version).IsModified = true;
                //db.Entry(instruction).Property(X => X.GroupId).IsModified = true;
                db.Entry(instruction).Property(X => X.Tag).IsModified = true;
                db.Entry(instruction).Property(X => X.TimeOfModification).IsModified = true;
                db.SaveChanges();

                if (instructionGroupViewModel.SelectedIds != null && instructionGroupViewModel.SelectedIds.Count() > 0)
                {
                    var wGroups = db.InstructionGroups.Where(w => w.InstructionId == instructionGroupViewModel.ID).ToList();

                    foreach (var item in instructionGroupViewModel.SelectedIds)
                    {
                        if ((wGroups.Where(x => x.InstructionId.Equals(instructionGroupViewModel.ID) && x.GroupId.Equals(Int32.Parse(item))).FirstOrDefault() == null) || wGroups.Count() == 0)
                        {
                            var groupInstructions = new InstructionGroup()
                            {
                                InstructionId = instruction.ID,
                                TimeOfCreation = DateTime.Now,
                                TimeOfModification = DateTime.Now,
                                GroupId = Int32.Parse(item)
                            };
                            db.InstructionGroups.Add(groupInstructions);
                            db.SaveChanges();
                        }
                    }
                    foreach (var item in wGroups)
                    {
                        if (!instructionGroupViewModel.SelectedIds.Contains(item.GroupId.ToString()))
                        {
                            db.InstructionGroups.Remove(item);
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    //Usuń wszystkie

                    var wGroups = db.InstructionGroups.Where(w => w.InstructionId == instructionGroupViewModel.ID);

                    foreach (var g in wGroups)
                    {
                        db.InstructionGroups.Remove(g);
                    }
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return View(instructionGroupViewModel);
        }

        // GET: Instructions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var instruction = db.Instructions.Find(id);

            if (instruction == null)
            {
                return HttpNotFound();
            }

            InstructionIndexData instructionGroupViewModel = new InstructionIndexData();
            var workerRepository = new WorkerRepository();
            var groups = workerRepository.GetAllGroups();

            instructionGroupViewModel.Name = instruction.Name;
            instructionGroupViewModel.Version = instruction.Version ;
            //instructionGroupViewModel.SelectedId = instruction.GroupId.ToString();

            instructionGroupViewModel.Items = groups.Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.GroupName
            });
            if (instructionGroupViewModel == null)
            {
                return HttpNotFound();
            }
            return View(instructionGroupViewModel);
        }

        // POST: Instructions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var instructionGroup = db.InstructionGroups.Where(x => x.InstructionId == id ).ToList();
            db.InstructionGroups.RemoveRange(instructionGroup);
            var instruction = db.Instructions.Find(id);
            db.Instructions.Remove(instruction);

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult AddNewVersion(int? id, string version, string training)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var instruction = db.Instructions.Find(id);

            if (instruction == null)
            {
                return HttpNotFound();
            }
            int presetVersion = instruction.Version;
            int newVersion = Int32.Parse(version);

            if (instruction.Version == Int32.Parse(version) || (newVersion != presetVersion + 1))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Instruction newInstruction = new Instruction();
            newInstruction.Name = instruction.Name;
            newInstruction.Version = Int32.Parse(version);
            newInstruction.TimeOfCreation = DateTime.Now;
            newInstruction.TimeOfModification = DateTime.Now;
            newInstruction.Number = instruction.Number;
            string currentUserId = User.Identity.GetUserId();
            newInstruction.CreatedByUserId = currentUserId;
            db.Instructions.Add(newInstruction);
            db.SaveChanges();

            if (!String.IsNullOrEmpty(training))
            { 
                TrainingName tn = new TrainingName();
                tn.Name = String.Empty;
                tn.Number = training;
                db.TrainingNames.Add(tn);
                db.SaveChanges();

                TrainingGroup tg = new TrainingGroup();
                tg.TrainingNameId = tn.ID;
                tg.InstructionId = newInstruction.ID;
                tg.TimeOfCreation = DateTime.Now;
                tg.TimeOfModification = DateTime.Now;

                db.TrainingGroups.Add(tg);
                db.SaveChanges();
            }

            var ids =
             (from InstructionGroups in db.InstructionGroups
             where
               InstructionGroups.GroupId != null &&
               InstructionGroups.InstructionId == id
             select new
             {
                 val = (InstructionGroups.GroupId ?? 0)
             }).Select(x => x.val.ToString()).ToList();

            if (ids != null && ids.Count() > 0)
            {
                foreach (var item in ids)
                {
                    var groupInstructions = new InstructionGroup()
                    {
                        InstructionId = newInstruction.ID,
                        TimeOfCreation = DateTime.Now,
                        TimeOfModification = DateTime.Now,
                        GroupId = Int32.Parse(item)
                    };
                    db.InstructionGroups.Add(groupInstructions);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Details", new { id = newInstruction.ID });
        }
        public ActionResult Excel(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


                var instructionVsTrainingList =
                             (from w in db.Workers
                              join wg in db.GroupWorkers on w.ID equals wg.WorkerId
                              join gi in db.InstructionGroups on wg.GroupId equals gi.GroupId
                              join i in db.Instructions on gi.InstructionId equals i.ID
                              join t in db.Trainings
                                    on new { InstructionId = i.ID, WorkerId = wg.WorkerId }
                                equals new { t.InstructionId, t.WorkerId } into t_join
                              from t in t_join.DefaultIfEmpty()
                              where i.ID == id
                              select new InstructionVsTrainingData
                              {
                                  WorkerLastName = w.LastName,
                                  WorkerFirstMidName = w.FirstMidName,
                                  WorkerFullName = w.LastName +  " " + w.FirstMidName,
                                  InstructionName = i.Name,
                                  GroupId = (int?)gi.GroupId,
                                  InstructionVersion = i.Version,
                                  InstructionNumber = i.Number,
                                  DateOfTraining = (DateTime?)t.DateOfTraining,
                                  TrainingName = t.TrainingName.Number
                              }).ToList();

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Trainings");
                ws.Cells["A1"].LoadFromCollection(instructionVsTrainingList, true);
                // Load your collection "accounts"

                Byte[] fileBytes = pck.GetAsByteArray();
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=DataTable.xlsx");
                // Replace filename with your custom Excel-sheet name.

                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";
                StringWriter sw = new StringWriter();
                Response.BinaryWrite(fileBytes);
                Response.End();
            }

            return RedirectToAction("Index");
        }

        
        public ActionResult Search(string srchterminstructions)
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
                     }).Where(ni => ni.Key.ToUpper().Contains(srchterminstructions.ToUpper()) || String.IsNullOrEmpty(srchterminstructions)).ToList();

            var allInstructions = db.Instructions.ToList().Where(x => newInstructions.Any(ni => ni.ID == x.ID) ).ToList();
            var workerRepository = new WorkerRepository();
            var groups = workerRepository.GetAllGroups();

            List<InstructionIndexData> lstInstructionGroups = new List<InstructionIndexData>();

            foreach (var item in allInstructions)
            {
                var instructioGroup = new InstructionIndexData();

                instructioGroup.ID = item.ID;
                instructioGroup.Name = item.Name;
                instructioGroup.Number = item.Number;
                instructioGroup.Version = item.Version;
                instructioGroup.UserName = item.CreatedByUserId;
                if (item.CreatedByUserId != null)
                    instructioGroup.UserName = db.Users.Find(item.CreatedByUserId).UserName;
                instructioGroup.TimeOfCreation = item.TimeOfCreation.ToShortDateString();

                var lstGroupIds = (from InstructionGroups in db.InstructionGroups
                                   where
                                     InstructionGroups.GroupId != null &&
                                     InstructionGroups.InstructionId == item.ID
                                   select new
                                   {
                                       val = (InstructionGroups.GroupId ?? 0)
                                   }).Select(x => x.val.ToString()).ToList();
                instructioGroup.SelectedIds = lstGroupIds.ToArray();

                instructioGroup.Items = groups.Select(x => new SelectListItem
                {
                    Value = x.ID.ToString(),
                    Text = x.GroupName
                });

                lstInstructionGroups.Add(instructioGroup);
            }

            return View("Index",lstInstructionGroups);
        }
    }
}