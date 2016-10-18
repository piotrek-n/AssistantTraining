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
            //string json = JsonConvert.SerializeObject(result);

            string json = @"{
        columns: [{
            title: ""NAME""
        }, {
            title: ""COUNTY""
        }],
        data: [
          [""John Doe"", ""Fresno""],
          [""Billy"", ""Fresno""],
          [""Tom"", ""Kern""],
          [""King Smith"", ""Kings""]
        ]
    }";

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