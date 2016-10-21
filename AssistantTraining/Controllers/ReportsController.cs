using AssistantTraining.Helpers;
using AssistantTraining.Repositories;
using AssistantTraining.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace AssistantTraining.Controllers
{
    public class ReportsController : Controller
    {
        private IGridMvcHelper gridMvcHelper;
        private const string GRID_PARTIAL_PATH = "~/Views/Reports/_ReportMainGrid.cshtml";

        public ReportsController()
        {
            this.gridMvcHelper = new GridMvcHelper();
        }

        // GET: Reports
        public ActionResult Index()
        {
            var appReports = new ReportsIndexData();
            List<SelectListItem> lstItems = new List<SelectListItem>();
            lstItems.Add(new SelectListItem()
            {
                Value = "0",
                Text = "Wyczyść"
            });
            lstItems.Add(new SelectListItem()
            {
                Value = "1",
                Text = "Lista szkoleń niekompletnych"
            });
            lstItems.Add(new SelectListItem()
            {
                Value = "2",
                Text = "Lista instrukcji, do których nie zostały utworzone szkolenia"
            });
            lstItems.Add(new SelectListItem()
            {
                Value = "3",
                Text = "Lista pracowników, którzy powinni odbyć jakiekolwiek szkolenie"
            });

            appReports.Items = lstItems;

            return View(appReports);
        }

        public ActionResult JsonAction(string q)
        {
            //https://www.sitepoint.com/working-jquery-datatables/
            //https://editor.datatables.net/examples/simple/inTableControls.html
            //https://datatables.net/examples/server_side/

            string json = string.Empty;
            switch (q)
            {
                case "0":
                    json = ReportsRepository.EmptyReport();
                    break;
                case "1":
                    json = ReportsRepository.IncompleteTraining();
                    break;
                case "2":
                    json = ReportsRepository.InstructionsWithoutTraining();
                    break;
                case "3":                    
                    json = ReportsRepository.WorkersWithoutTraining();
                    break;
                default:
                    break;
            }

            JavaScriptSerializer j = new JavaScriptSerializer();
            object a = j.Deserialize(json, typeof(object));

            return Json(a, JsonRequestBehavior.AllowGet);
        }

        [AjaxChildActionOnly]
        public PartialViewResult GetGrid(string term)
        {
            var repos = new WorkerRepository();
            var items = repos.GetTrainings().OrderBy(p => 0);
            var grid = this.gridMvcHelper.GetAjaxGrid(items);

            return PartialView(GRID_PARTIAL_PATH, grid);
        }

        public ActionResult GetFooString()
        {
            String Foo = "This is my foo string.";
            return Json(Foo, JsonRequestBehavior.AllowGet);
        }
    }
}