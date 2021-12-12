using System.Linq;
using System.Web.Mvc;
using AssistantTraining.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;

namespace AssistantTraining.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult Select([DataSourceRequest]DataSourceRequest request)
        {
            var data = Enumerable.Range(1, 10)
                .Select(index => new Worker
                {
                    LastName = "Product #" + index,
                    FirstMidName = (index * 10).ToString(),
                    Tag = "Tag" + index
                });

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
    }
}