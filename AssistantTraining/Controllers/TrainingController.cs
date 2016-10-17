using AssistantTraining.DAL;
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
        public ActionResult UpdateTrainings(TrainingUpdateData model)
        {
            if (model != null)
            {
                if (model.Workers != null)
                {
                    foreach (var w in model.Workers)
                    {
                        var tr = db.Trainings.Where(x => x.TrainingNameId.Equals(w.TrainingNameId) && x.WorkerId.Equals(w.WorkerID)).FirstOrDefault();

                        if (tr == null && w.Checked.Equals(true))
                        {
                            DateTime dt = DateTime.ParseExact(model.TrainingDate, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                            var instruction = db.TrainingGroups.Where(x => x.TrainingNameId.Equals(w.TrainingNameId)).FirstOrDefault();
                            if (instruction != null)
                            {
                                Training tn = new Training();
                                tn.WorkerId = w.WorkerID;
                                tn.TrainingNameId = w.TrainingNameId;
                                tn.TimeOfCreation = DateTime.Now;
                                tn.TimeOfModification = DateTime.Now;
                                tn.DateOfTraining = dt;
                                tn.InstructionId = instruction.InstructionId;
                                db.Trainings.Add(tn);
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

        public ActionResult Excel()
        {
            if (Session["term"] == null || Session["type"] == null || String.IsNullOrEmpty(Session["term"].ToString()) || String.IsNullOrEmpty(Session["type"].ToString()))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var repos = new WorkerRepository();

            var workers = repos.GetWorkersByTraining(Session["term"].ToString(), Session["type"].ToString()).Select(x => new { Name = x.WorkerLastName, FirstName = x.WorkerFirstMidName });

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Workers");
                ws.Cells["A1"].LoadFromCollection(workers, true);
                // Load your collection "accounts"

                Byte[] fileBytes = pck.GetAsByteArray();
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=Workers.xlsx");
                // Replace filename with your custom Excel-sheet name.

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
            List<InstructionsJson> lstInstructions = new List<InstructionsJson>();
            if (t.Equals("true"))
            {
                lstInstructions = db.Instructions.Where(x => x.Number.Contains(q)).Select(x => new InstructionsJson { id = x.ID.ToString(), text = x.Number,  name = x.Name }).ToList();
            }
            else
            {
              
                lstInstructions = (from i in db.Instructions
                                   join tg in db.TrainingGroups on new { ID = i.ID } equals new { ID = tg.InstructionId } into tg_join
                                   from tg in tg_join.DefaultIfEmpty()
                                   where
                                     i.Number.Contains(q) &&
                                     tg.InstructionId == null
                                   select new InstructionsJson { id = i.ID.ToString(), text = i.Number,name = i.Name }).ToList();
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
        public ActionResult AddNewTrainings(string selectedValues,string trainingNumber)
        {
            if (selectedValues.Length > 0)
            {
                string[] values = selectedValues.Split(',');
                foreach (var val in values)
                {
                    var countTrainingNames = db.TrainingNames.Where(x => x.Number.ToLower().Equals(trainingNumber.Trim().ToLower())).Count();
                    if (countTrainingNames == 0)
                    {
                        TrainingName tn = new TrainingName();
                        tn.Name = String.Empty;
                        tn.Number = trainingNumber;
                        db.TrainingNames.Add(tn);
                        db.SaveChanges();

                        TrainingGroup tg = new TrainingGroup();
                        tg.TrainingNameId = tn.ID;
                        tg.InstructionId = Int32.Parse(val);
                        tg.TimeOfCreation = DateTime.Now;
                        tg.TimeOfModification = DateTime.Now;
                        db.TrainingGroups.Add(tg);
                        db.SaveChanges();
                    }
                }
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
    }
}