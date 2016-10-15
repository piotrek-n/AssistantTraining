using AssistantTraining.DAL;
using AssistantTraining.Models;
using AssistantTraining.ViewModel;
using System;
using System.Collections.Generic;
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
            return View(db.Groups.ToList());
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

            var lst = (from ig in db.InstructionGroups where ig.GroupId == id
                       join i in db.Instructions on ig.InstructionId equals i.ID
                       select i).ToList();
            if (lst != null && lst.Count > 0)
            {
                gd.Instructions = new List<InstructionInGroup>();
                foreach (var item in lst)
                {
                    gd.Instructions.Add(new InstructionInGroup() { Name = item.Name, Number = item.Number });
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