using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace NewFeature.Controllers
{
    public class Select2Controller : Controller
    {
        // GET: Select2
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SomeJsonAction(string q)
        {
            var str = @"{""total_count"":2,""items"": [    {""id"":""01"", ""text"":""item 1""},    {""id"":""02"", ""text"":""item 2""}]}";
            JavaScriptSerializer j = new JavaScriptSerializer();
            object a = j.Deserialize(str, typeof(object));

            return Json(a, JsonRequestBehavior.AllowGet);
        }
    }
}