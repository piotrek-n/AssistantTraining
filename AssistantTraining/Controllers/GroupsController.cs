using AssistantTraining.DAL;
using AssistantTraining.Models;
using AssistantTraining.Repositories;
using AssistantTraining.ViewModel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace AssistantTraining.Controllers
{
    [Authorize]
    public class GroupsController : Controller
    {
        private AssistantTrainingContext db = new AssistantTrainingContext();

        // GET: Groups
        public ActionResult Index()
        {
            //LINQ to Entities does not recognize the method 'System.Linq.IQueryable`
            //FIX Select(x => x).AsEnumerable()
            var result =
                db.Groups.Select(x => x).AsEnumerable().Select((x, index) => new GroupViewModel()
                {
                    RowNo = index + 1,
                    GroupName = x.GroupName,
                    ID = x.ID,
                    Instructions = x.Instructions,
                    Tag = x.Tag,
                    TimeOfCreation = x.TimeOfCreation,
                    TimeOfModification = x.TimeOfModification
                }).ToList();

            return View(result);
        }

        // GET: Groups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            GroupDetails gd = new GroupDetails();
            gd.ID = group.ID;
            gd.GroupName = group.GroupName;

            //
            var items = (
                from i in db.Instructions
                group i by i.Number into groupedI
                let maxVersion = groupedI.Max(gt => gt.Version)
                select new
                {
                    Key = groupedI.Key,
                    ID = groupedI.FirstOrDefault(gt2 => gt2.Version == maxVersion).ID
                }
            ).Select(x=>x.ID).ToList();
            
            var lst = (from ig in db.InstructionGroups
                        join i in db.Instructions on ig.InstructionId equals i.ID
                        where ig.GroupId == id && items.Contains(i.ID)
                        select i).ToList();
            
            if (lst != null && lst.Count > 0)
            {
                gd.Instructions = new List<InstructionInGroup>();
                foreach (var item in lst)
                {
                    gd.Instructions.Add(new InstructionInGroup() { Name = item.Name, Number = item.Number , Version = item.Version, ID = item.ID});
                }
            }

            return View(gd);
        }

        // GET: Groups/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Groups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,GroupName,TimeOfCreation,TimeOfModification,Tag")] Group group)
        {
            if (ModelState.IsValid)
            {
                group.TimeOfCreation = DateTime.Now;
                group.TimeOfModification = DateTime.Now;

                db.Groups.Add(group);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(group);
        }

        // GET: Groups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        // POST: Groups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,GroupName,TimeOfCreation,TimeOfModification,Tag")] Group group)
        {
            if (ModelState.IsValid)
            {
                group.TimeOfModification = DateTime.Now;

                db.Groups.Attach(group);
                db.Entry(group).Property(X => X.GroupName).IsModified = true;
                db.Entry(group).Property(X => X.Tag).IsModified = true;
                db.Entry(group).Property(X => X.TimeOfModification).IsModified = true;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(group);
        }

        // GET: Groups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Group group = db.Groups.Find(id);
            db.Groups.Remove(group);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult SearchByGroup(string srchtermWorkerByGroup)
        {
            //var items = db.Groups.Where(x => x.GroupName.ToUpper().Contains(srchtermWorkerByGroup.ToUpper())).ToList();

            ////LINQ to Entities does not recognize the method 'System.Linq.IQueryable`
            ////FIX Select(x => x).AsEnumerable()
            var result =
                db.Groups.Where(x => x.GroupName.ToUpper().Contains(srchtermWorkerByGroup.ToUpper())).Select(x => x).AsEnumerable().Select((x, index) => new GroupViewModel()
                {
                    RowNo = index + 1,
                    GroupName = x.GroupName,
                    ID = x.ID,
                    Instructions = x.Instructions,
                    Tag = x.Tag,
                    TimeOfCreation = x.TimeOfCreation,
                    TimeOfModification = x.TimeOfModification
                }).ToList();
            //return View("Groups/Index", result);
            //LINQ to Entities does not recognize the method 'System.Linq.IQueryable`
            //FIX Select(x => x).AsEnumerable()

            return View("Index", result);
        }

        public ActionResult Excel()
        {
            var groups = db.Groups.Select(x => new { Name = x.GroupName }).ToList();

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Groups");
                ws.Cells["A1"].LoadFromCollection(groups, true);


                Byte[] fileBytes = pck.GetAsByteArray();
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=Groups.xlsx");

                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";
                StringWriter sw = new StringWriter();
                Response.BinaryWrite(fileBytes);
                Response.End();
            }

            return RedirectToAction("Index");
        }

        public ActionResult TrainedUsers(int id)
        {
            var workers = (from t in db.Trainings
                           join w in db.Workers on t.WorkerId equals w.ID
                           where t.InstructionId.Equals(id) && t.DateOfTraining > new DateTime(2000 , 1, 1)
                           select w);


            return View(workers);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}