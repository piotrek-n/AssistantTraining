using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AssistantTraining.DAL;
using AssistantTraining.Models;

namespace AssistantTraining.Controllers
{
    public class GroupInstructionsController : Controller
    {
        private AssistantTrainingContext db = new AssistantTrainingContext();

        // GET: GroupInstructions
        public ActionResult Index()
        {
            return View(db.GroupInstructions.ToList());
        }

        // GET: GroupInstructions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupInstruction groupInstruction = db.GroupInstructions.Find(id);
            if (groupInstruction == null)
            {
                return HttpNotFound();
            }
            return View(groupInstruction);
        }

        // GET: GroupInstructions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GroupInstructions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,GroupName,TimeOfCreation,TimeOfModification,Tag")] GroupInstruction groupInstruction)
        {
            if (ModelState.IsValid)
            {
                groupInstruction.TimeOfCreation = DateTime.Now;
                groupInstruction.TimeOfModification = DateTime.Now;

                db.GroupInstructions.Add(groupInstruction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(groupInstruction);
        }

        // GET: GroupInstructions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupInstruction groupInstruction = db.GroupInstructions.Find(id);
            if (groupInstruction == null)
            {
                return HttpNotFound();
            }
            return View(groupInstruction);
        }

        // POST: GroupInstructions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,GroupName,TimeOfCreation,TimeOfModification,Tag")] GroupInstruction groupInstruction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(groupInstruction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(groupInstruction);
        }

        // GET: GroupInstructions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupInstruction groupInstruction = db.GroupInstructions.Find(id);
            if (groupInstruction == null)
            {
                return HttpNotFound();
            }
            return View(groupInstruction);
        }

        // POST: GroupInstructions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GroupInstruction groupInstruction = db.GroupInstructions.Find(id);
            db.GroupInstructions.Remove(groupInstruction);
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
