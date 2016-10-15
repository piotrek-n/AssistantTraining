using AssistantTraining.Helpers;
using AssistantTraining.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
    }
}