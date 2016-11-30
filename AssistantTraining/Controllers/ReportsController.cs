using AssistantTraining.Helpers;
using AssistantTraining.Repositories;
using AssistantTraining.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
                Text = "SZKOLENIA"
            });
            lstItems.Add(new SelectListItem()
            {
                Value = "2",
                Text = "INSTRUKCJE"
            });
            lstItems.Add(new SelectListItem()
            {
                Value = "3",
                Text = "PRACOWNICY"
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

        public ActionResult Excel2(int id)
        {
            string json = null;
            switch (id)
            {
                case 1:
                    json = ReportsRepository.IncompleteTrainingJSON();
                    break;

                case 2:
                    json = ReportsRepository.InstructionsWithoutTrainingJSON();
                    break;

                case 3:
                    json = ReportsRepository.WorkersWithoutTrainingJSON();
                    break;

                case 0:
                default:
                    break;
            }
            if (String.IsNullOrEmpty(json))
                return null;
            //DataTable dt = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));
            DataTable dt = Tabulate(json);
            using (ExcelPackage pck = new ExcelPackage())
            {
                try
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Workers");
                    ws.Cells["A1"].LoadFromDataTable(dt, true);
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
                catch (System.Web.HttpException ex)
                {
                    if (ex.Message.StartsWith("The remote host closed the connection.")) ;//do nothing

                    //handle other errors
                }
                catch (Exception e)
                {
                    //handle other errors
                }
                finally
                {//close streams etc..
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult Excel(int id)
        {
            string fileName = String.Empty;

            string json = null;

            GenerateReport(id, ref fileName, ref json);

            if (String.IsNullOrEmpty(json))
                return null;

            DataTable dt = Tabulate(json);
            using (ExcelPackage pck = new ExcelPackage())
            {
                try
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Workers");
                    ws.Cells["A1"].LoadFromDataTable(dt, true);

                    Byte[] fileBytes = pck.GetAsByteArray();
                    Response.Clear();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".xlsx");

                    Response.Charset = "";
                    Response.ContentType = "application/vnd.ms-excel";
                    StringWriter sw = new StringWriter();
                    Response.BinaryWrite(fileBytes);
                    Response.End();
                }
                catch (System.Web.HttpException ex)
                {
                    if (ex.Message.StartsWith("The remote host closed the connection.")) { }
                }
                catch (Exception e)
                {
                }
                finally
                {
                }
            }

            return RedirectToAction("Index");
        }

        private static void GenerateReport(int id, ref string fileName, ref string json)
        {
            switch (id)
            {
                case 1:
                    json = ReportsRepository.IncompleteTrainingJSON();
                    fileName = "IncompleteTrainings_" + DateTime.Now.ToLongDateString();
                    break;

                case 2:
                    json = ReportsRepository.InstructionsWithoutTrainingJSON();
                    fileName = "InstructionsWithoutTraining_" + DateTime.Now.ToLongDateString();
                    break;

                case 3:
                    json = ReportsRepository.WorkersWithoutTrainingJSON();
                    fileName = "WorkersWithoutTraining_" + DateTime.Now.ToLongDateString();
                    break;

                case 0:
                default:
                    break;
            }
        }

        public static DataTable Tabulate(string json)
        {
            var jsonLinq = JObject.Parse(json);

            // Find the first array using Linq
            var srcArray = jsonLinq.Descendants().Where(d => d is JArray).First();
            var trgArray = new JArray();
            foreach (JObject row in srcArray.Children<JObject>())
            {
                var cleanRow = new JObject();
                foreach (JProperty column in row.Properties())
                {
                    // Only include JValue types
                    if (column.Value is JValue)
                    {
                        cleanRow.Add(column.Name, column.Value);
                    }
                }

                trgArray.Add(cleanRow);
            }

            return JsonConvert.DeserializeObject<DataTable>(trgArray.ToString());
        }
    }
}