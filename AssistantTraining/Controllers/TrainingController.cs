﻿using AssistantTraining.DAL;
using AssistantTraining.Helpers;
using AssistantTraining.Models;
using AssistantTraining.Repositories;
using AssistantTraining.ViewModel;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace AssistantTraining.Controllers
{
    [Authorize]
    public class TrainingController : Controller
    {
        /* Staff Training*/
        /*z jakich instrukcji pracownik powinien być przeszkolony i czy te szkolenia się odbyły*/
        /*
         * W1
         *  I1 Przeszkolony: Tak/Nie T.Date
         *  I2 Przeszkolony: Tak/Nie T.Date
         * W2
         *  I1 Przeszkolony: Tak/Nie T.Date
         *  I2 Przeszkolony: Tak/Nie T.Date
         *
                Select * from [dbo].[Workers] w
                LEFT JOIN [dbo].[GroupInstructions] gi ON w.ID = gi.WorkerID
                LEFT JOIN [dbo].[Instructions] i ON gi.GroupID = i.GroupID
                LEFT JOIN [dbo].[Trainings] t ON t.WorkerId = w.ID AND t.[InstructionId] = i.ID
                order BY w.LastName, w.FirstMidName
         */

        /* Training */
        /*śledzenie instrukcji – jacy pracownicy są podpięci do tych instrukcji i którzy wymagają przeszkolenia*/
        /*
         * I1
         *  W1 Przeszkolony: Tak/Nie
         *  W2 Przeszkolony: Tak/Nie
         * I12
         *  W1 Przeszkolony: Tak/Nie
         *  W2 Przeszkolony: Tak/Nie
         */

        private AssistantTrainingContext db = new AssistantTrainingContext();

        //public ActionResult Index(int? id)
        //{
        //    var viewModel = new TrainingIndexData();

        //    //var lst = (from w in db.Workers
        //    //           join gw in db.GroupWorkers on w.ID equals gw.WorkerId
        //    //           join ig in db.InstructionGroups on new { GroupId = gw.GroupId } equals new { GroupId = Convert.ToInt32(ig.GroupId) }
        //    //           join i in db.Instructions on ig.InstructionId equals i.ID
        //    //           join t in db.Trainings on new { wID = w.ID , iID = i.ID } equals new { wID = t.WorkerId, iID = t.InstructionId }
        //    //           into gj
        //    //           from e in gj.DefaultIfEmpty()
        //    //           select new TrainingItemIndexData{ WorkId=w.ID, Worker =w, Instruction =i, Training =e}
        //    //           //select new {w, i= (new InstructionExt(w.ID,i)),e = (new TrainingExt{ WorkerID = w.ID }) }
        //    //           ).OrderBy(x=>x.Worker.LastName).ToList();
        //    viewModel.items = null;

        //    return View(viewModel);
        //}

        public JsonResult GetInstructionsByQuery(string query)
        {
            return Json((from i in db.Instructions where i.Number.Contains(query) select i.Number).Distinct().ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTrainingNamesByQuery(string query)
        {
            return Json((from i in db.TrainingNames where i.Number.Contains(query) select i.Number).Distinct().ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Search(string instruction, string training)
        {
            var viewModel = new TrainingIndexData();
            return View(viewModel);
        }

        private const string GRID_PARTIAL_PATH = "~/Views/Training/_TrainingGrid.cshtml";
        private const string GRID_WORKER_PARTIAL_PATH = "~/Views/Training/_TrainingWorkersGrid.cshtml";

        private IGridMvcHelper gridMvcHelper;

        public TrainingController()
        {
            this.gridMvcHelper = new GridMvcHelper();
        }

        public ActionResult Index()
        {
            return View();
        }

        [AjaxChildActionOnly]
        public PartialViewResult GetGrid(string term)
        {
            var repos = new WorkerRepository();
            var items = repos.GetTrainings().OrderBy(p => 0);
            var grid = this.gridMvcHelper.GetAjaxGrid(items);

            return PartialView(GRID_PARTIAL_PATH, grid);
        }

        [AjaxChildActionOnly]
        public PartialViewResult GetGridByInstruction(string term)
        {
            var repos = new WorkerRepository();
            var items = repos.GetTrainings().Where(x => x.Instruction.Number.Contains(term)).OrderBy(p => 0);
            var grid = this.gridMvcHelper.GetAjaxGrid(items);

            return PartialView(GRID_PARTIAL_PATH, grid);
        }

        [AjaxChildActionOnly]
        public PartialViewResult GetGridByTraining(string term)
        {
            var repos = new WorkerRepository();
            var items = repos.GetTrainings().Where(x => x.TrainingName.Number.Contains(term)).OrderBy(p => 0);
            var grid = this.gridMvcHelper.GetAjaxGrid(items);

            return PartialView(GRID_PARTIAL_PATH, grid);
        }

        [AjaxChildActionOnly]
        public PartialViewResult GetWorkerGrid(string term, string type)
        {
            var repos = new WorkerRepository();
            Session["term"] = term;
            Session["type"] = type;

            var items = repos.GetWorkersByTraining(term, type).OrderBy(p => 0);
            var grid = this.gridMvcHelper.GetAjaxGrid(items);

            return PartialView(GRID_WORKER_PARTIAL_PATH, grid);
        }

        [HttpGet]
        public ActionResult GridPager(int? page)
        {
            var repos = new WorkerRepository();
            var items = repos.GetTrainings().OrderBy(p => 0);
            var grid = this.gridMvcHelper.GetAjaxGrid(items, page);
            object jsonData = this.gridMvcHelper.GetGridJsonData(grid, GRID_PARTIAL_PATH, this);

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GridWorkerPager(int? page, string a)
        {
            var repos = new WorkerRepository();
            var term = Session["term"] as string;
            var type = Session["type"] as string;
            var items = repos.GetWorkersByTraining(term, type).OrderBy(p => 0);
            var grid = this.gridMvcHelper.GetAjaxGrid(items, page);
            object jsonData = this.gridMvcHelper.GetGridJsonData(grid, GRID_WORKER_PARTIAL_PATH, this);

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [AjaxChildActionOnly]
        public ActionResult RemoveTrainings(TrainingUpdateData model)
        {
            if (model.Workers != null)
            {
                foreach (var w in model.Workers)
                {
                    //Czy zaznaczeniej do pojedynczej instrukcji traktujemu jako zaznaczenie szkolenia do tej instrukcji, czy do wszystkich instrukcji
                    //z tego szkolenia.
                    var tr = db.Trainings.Where(x => x.TrainingNameId.Equals(w.TrainingNameId) && x.WorkerId.Equals(w.WorkerID)).ToList();

                    if (tr != null && w.Checked.Equals(true) && tr.Count > 0)
                    {
                        foreach (var item in tr)
                        {
                            db.Trainings.Attach(item);
                            db.Trainings.Remove(item);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        var b = 0;
                    }
                }
            }

            var repos = new WorkerRepository();
            var term = Session["term"] as string;
            var type = Session["type"] as string;
            var items = repos.GetWorkersByTraining(term, type).OrderBy(p => 0);
            var grid = this.gridMvcHelper.GetAjaxGrid(items);
            return PartialView(GRID_WORKER_PARTIAL_PATH, grid);
        }

        [AjaxChildActionOnly]
        public ActionResult UpdateTrainings(TrainingUpdateData model)
        {
            if (model != null)
            {
                if (model.Workers != null)
                {
                    foreach (var w in model.Workers)
                    {
                        //Czy zaznaczeniej do pojedynczej instrukcji traktujemu jako zaznaczenie szkolenia do tej instrukcji, czy do wszystkich instrukcji
                        //z tego szkolenia.
                        var tr = db.Trainings.Where(x => x.TrainingNameId.Equals(w.TrainingNameId) && x.WorkerId.Equals(w.WorkerID)).ToList();

                        if (tr != null && w.Checked.Equals(true) && tr.Count > 0)
                        {
                            DateTime dt = DateTime.ParseExact(model.TrainingDate, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                            foreach (var item in tr)
                            {
                                item.DateOfTraining = dt;
                                db.Entry(item).Property(X => X.DateOfTraining).IsModified = true;
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            var b = 0;
                        }
                    }
                }
            }

            var repos = new WorkerRepository();
            var term = Session["term"] as string;
            var type = Session["type"] as string;
            var items = repos.GetWorkersByTraining(term, type).OrderBy(p => 0);
            var grid = this.gridMvcHelper.GetAjaxGrid(items);
            return PartialView(GRID_WORKER_PARTIAL_PATH, grid);
        }

        public ActionResult DeleteTraining(int id)
        {
            try
            {
                var tr = db.Trainings.Where(x => x.TrainingNameId.Equals(id)).ToList();

                if (tr != null && tr.Count > 0)
                {
                    foreach (var item in tr)
                    {
                        db.Trainings.Attach(item);
                        db.Trainings.Remove(item);
                        //db.SaveChanges();
                    }
                }

                var tg = db.TrainingGroups.Where(x => x.TrainingNameId.Equals(id)).ToList();
                if (tg != null && tg.Count > 0)
                {
                    foreach (var item in tg)
                    {
                        db.TrainingGroups.Attach(item);
                        db.TrainingGroups.Remove(item);
                        //db.SaveChanges();
                    }
                }
                var tn = db.TrainingNames.Where(x => x.ID.Equals(id)).ToList();

                if (tn != null && tn.Count > 0)
                {
                    foreach (var item in tn)
                    {
                        db.TrainingNames.Attach(item);
                        db.TrainingNames.Remove(item);
                        //db.SaveChanges();
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                //Handle Exception;
                return View("Error");
            }
        }

        public ActionResult Excel()
        {
            if (Session["term"] == null || Session["type"] == null || String.IsNullOrEmpty(Session["term"].ToString()) || String.IsNullOrEmpty(Session["type"].ToString()))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var repos = new WorkerRepository();

            var workers = repos.GetWorkersByTraining(Session["term"].ToString(), Session["type"].ToString()).Select(x => new { Name = x.WorkerLastName, FirstName = x.WorkerFirstMidName });

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Workers");
                ws.Cells["A1"].LoadFromCollection(workers.Select(x => new { FullName = x.FirstName + " " + x.Name }).ToList(), true);

                Byte[] fileBytes = pck.GetAsByteArray();
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=Workers.xlsx");

                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";
                StringWriter sw = new StringWriter();
                Response.BinaryWrite(fileBytes);
                Response.End();
            }

            return RedirectToAction("Index");
        }

        public ActionResult InstructionsJsonAction(string q, string t)
        {
            Session["term"] = t;
            List<InstructionsJson> lstInstructions = new List<InstructionsJson>();
            if (t.Equals("true"))
            {
                lstInstructions = (
                 from i in db.Instructions
                 group i by i.Number into groupedI
                 let maxVersion = groupedI.Max(gt => gt.Version)
                 select new
                 {
                     Key = groupedI.Key,
                     ID = groupedI.FirstOrDefault(gt2 => gt2.Version == maxVersion).ID,
                     Number = groupedI.FirstOrDefault(gt2 => gt2.Version == maxVersion).Number,
                     Name = groupedI.FirstOrDefault(gt2 => gt2.Version == maxVersion).Name,
                     Version = groupedI.FirstOrDefault(gt2 => gt2.Version == maxVersion).Version
                 }
               ).Select(x => new InstructionsJson { id = x.ID.ToString(), text = x.Number, name = x.Name, version = x.Version })
               .Where(x => x.text.Contains(q))
               .ToList();
            }
            else
            {
                var items = (
                          from i in db.Instructions
                          group i by i.Number into groupedI
                          let maxVersion = groupedI.Max(gt => gt.Version)
                          select new
                          {
                              Key = groupedI.Key,
                              ID = groupedI.FirstOrDefault(gt2 => gt2.Version == maxVersion).ID
                          }
                      ).Select(x => x.ID).ToList();

                lstInstructions = (from i in db.Instructions
                                   join tg in db.TrainingGroups on new { ID = i.ID } equals new { ID = tg.InstructionId } into tg_join
                                   from tg in tg_join.DefaultIfEmpty()
                                   where
                                     i.Number.Contains(q)
                                     && items.Contains(i.ID)
                                     && tg.InstructionId == null
                                   select new InstructionsJson { id = i.ID.ToString(), text = i.Number, name = i.Name, version = i.Version }).ToList();
                //**FIX
                //If a new worker was added.
                if (lstInstructions.Count() == 0)
                {
                    lstInstructions =
                   (from w in db.Workers
                    join wg in db.GroupWorkers on w.ID equals wg.WorkerId
                    join gi in db.InstructionGroups on wg.GroupId equals gi.GroupId
                    join i in db.Instructions on gi.InstructionId equals i.ID
                    join tt in db.Trainings
                          on new { InstructionId = i.ID, WorkerId = wg.WorkerId }
                      equals new { tt.InstructionId, tt.WorkerId } into t_join
                    from tt in t_join.DefaultIfEmpty()
                    where
                      w.IsSuspend == false
                      && items.Contains(i.ID)
                    select new InstructionsJson { id = i.ID.ToString(), text = i.Number, name = i.Name, version = i.Version }).Distinct().ToList();

                }
            }
            var countInstructions = lstInstructions.Count();

            InstructionsJsonDTO result = new InstructionsJsonDTO();
            if (countInstructions > 0)
            {
                result.total_count = countInstructions.ToString();
                result.items = lstInstructions;
            }

            string json = JsonConvert.SerializeObject(result);

            JavaScriptSerializer j = new JavaScriptSerializer();
            object a = j.Deserialize(json, typeof(object));

            return Json(a, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddNewTrainings(string selectedValues, string trainingNumber)
        {
            if (selectedValues.Length > 0)
            {
                string[] selectedValuesInstruction = selectedValues.Split(',');
                var intInstructionIDs = selectedValuesInstruction.Select(int.Parse).ToList();

                #region Add new training

                TrainingName tn = new TrainingName();
                tn.Name = String.Empty;
                tn.Number = trainingNumber;
                db.TrainingNames.Add(tn);
                db.SaveChanges();

                #endregion Add new training

                #region Add TrainingGroup

                foreach (var val in selectedValuesInstruction)
                {
                    TrainingGroup tg = new TrainingGroup();
                    tg.TrainingNameId = tn.ID;
                    tg.InstructionId = Int32.Parse(val);
                    tg.TimeOfCreation = DateTime.Now;
                    tg.TimeOfModification = DateTime.Now;
                    db.TrainingGroups.Add(tg);
                    db.SaveChanges();
                }

                #endregion Add TrainingGroup

                #region Add all workers and assigned instruction per TrainingGroup


                //select* from[dbo].[Workers] w
                //inner join[dbo].[GroupWorkers] gw on gw.[WorkerId] = w.ID
                //inner join dbo.InstructionGroups ig on ig.[GroupId] = gw.[GroupId]

                var instructionWorkerList =
                   (from w in db.Workers
                    join gw in db.GroupWorkers on w.ID equals gw.WorkerId
                    join ig in db.InstructionGroups on gw.GroupId  equals  ig.GroupId
                    join t in db.Trainings
                          on new { WorkerId = w.ID, ID = ig.Instruction.ID }
                      equals new { t.WorkerId, ID = t.InstructionId } into t_join
                    from t in t_join.DefaultIfEmpty()
                    where
                    w.IsSuspend == false && intInstructionIDs.Contains(ig.Instruction.ID) && (int?)t.ID == null
                    select new
                    {
                        WorkerID = w.ID,
                        InstructionID = ig.Instruction.ID
                    }
                  ).ToList();

                if (instructionWorkerList != null)
                {
                    foreach (var instruction in instructionWorkerList)
                    {
                        if (instruction != null)
                        {
                            Training newTraining = new Training();
                            newTraining.WorkerId = instruction.WorkerID;
                            newTraining.TrainingNameId = tn.ID;
                            newTraining.TimeOfCreation = DateTime.Now;
                            newTraining.TimeOfModification = DateTime.Now;
                            newTraining.DateOfTraining = new DateTime(1900, 1, 1);
                            newTraining.InstructionId = instruction.InstructionID;
                            db.Trainings.Add(newTraining);
                            db.SaveChanges();
                        }
                    }
                }

                #endregion Add all workers and assigned instruction per TrainingGroup
            }

            return Json("success");
        }
    }

    public class InstructionsJsonDTO
    {
        public string total_count;
        public List<InstructionsJson> items;
    }

    public class InstructionsJson
    {
        public string id;
        public string text;
        public string name;
        public string version;
    }
}