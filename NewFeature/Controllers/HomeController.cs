using NewFeature.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewFeature.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetJsonData(string query)
        {
            List<string> tab = new List<string>();
            tab.Add("aaa");
            tab.Add("bbb");
            tab.Add("ccc");

            return Json(tab.Where(s => s.Contains(query)), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxMethod(string name)
        {
            PersonModel person = new PersonModel
            {
                Name = name,
                DateTime = DateTime.Now.ToString()
            };
            return Json(person);
        }

    }
}