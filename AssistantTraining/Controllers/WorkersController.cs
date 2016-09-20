using AssistantTraining.DAL;
using AssistantTraining.Models;
using AssistantTraining.Repositories;
using AssistantTraining.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace AssistantTraining.Controllers
{
    public class WorkersController : Controller
    {
        private AssistantTrainingContext db = new AssistantTrainingContext();

        // GET: Workers
        public ActionResult Index()
        {
            var allWorker = db.Workers.ToList();
            var workerRepository = new WorkerRepository();
            var groups = workerRepository.GetAllGroups();

            List<WorkerGroupViewModel> lstWorkerGroups = new List<WorkerGroupViewModel>();

            foreach (var item in allWorker)
            {
                var workerGroup = new WorkerGroupViewModel();

                workerGroup.ID = item.ID;
                workerGroup.FirstMidName = item.FirstMidName;
                workerGroup.LastName = item.LastName;
                workerGroup.Tag = item.Tag;
                workerGroup.SelectedIds = db.GroupInstructions.Where(x => x.WorkerId.Equals(item.ID)).Select(x => x.GroupId.ToString()).ToArray();
                workerGroup.WorkerGroups = groups;
                workerGroup.Items = groups.Select(x => new SelectListItem
                {
                    Value = x.ID.ToString(),
                    Text = x.GroupName
                });

                lstWorkerGroups.Add(workerGroup);
            }
            return View(lstWorkerGroups);
        }

        // GET: Workers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Worker worker = db.Workers.Find(id);
            if (worker == null)
            {
                return HttpNotFound();
            }
            return View(worker);
        }

        // GET: Workers/Create
        public ActionResult Create()
        {
            //var groups = db.Groups.Select(c => new
            //{
            //    GroupID = c.ID,
            //    GroupName = c.GroupName
            //}).ToList();
            //ViewBag.Groups = new MultiSelectList(groups, "GroupID", "GroupName");

            var workerGroup = new WorkerGroupViewModel();
            var workerRepository = new WorkerRepository();
            var groups = workerRepository.GetAllGroups();

            workerGroup.WorkerGroups = groups;
            workerGroup.Items = groups.Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.GroupName
            });

            return View(workerGroup);
        }

        // POST: Workers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(WorkerGroupViewModel workerGroup)
        {
            if (ModelState.IsValid)
            {
                var worker = new Worker();
                worker.TimeOfCreation = DateTime.Now;
                worker.TimeOfModification = DateTime.Now;
                worker.LastName = workerGroup.LastName;
                worker.FirstMidName = workerGroup.FirstMidName;
                worker.Tag = workerGroup.Tag;

                db.Workers.Add(worker);
                db.SaveChanges();

                if (workerGroup.SelectedIds != null && workerGroup.SelectedIds.Count() > 0)
                {
                    foreach (var item in workerGroup.SelectedIds)
                    {
                        var groupInstructions = new GroupInstruction()
                        {
                            WorkerId = worker.ID,
                            TimeOfCreation = DateTime.Now,
                            TimeOfModification = DateTime.Now,
                            GroupId = Int32.Parse(item)
                        };
                        db.GroupInstructions.Add(groupInstructions);
                        db.SaveChanges();
                    }
                }

                return RedirectToAction("Index");
            }

            return View(workerGroup);
        }

        // GET: Workers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Worker worker = db.Workers.Find(id);

            var workerGroup = new WorkerGroupViewModel();
            var workerRepository = new WorkerRepository();
            var groups = workerRepository.GetAllGroups();

            workerGroup.WorkerGroups = groups;
            workerGroup.Items = groups.Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.GroupName
            });

            workerGroup.FirstMidName = worker.FirstMidName;
            workerGroup.LastName = worker.LastName;
            workerGroup.ID = worker.ID;
            workerGroup.Tag = worker.Tag;
            workerGroup.SelectedIds = db.GroupInstructions.Where(x => x.WorkerId.Equals(worker.ID)).Select(x => x.GroupId.ToString()).ToArray();

            if (worker == null)
            {
                return HttpNotFound();
            }
            return View(workerGroup);
        }

        // POST: Workers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(WorkerGroupViewModel workerGroup)
        {
            if (ModelState.IsValid)
            {
                var worker = new Worker();
                worker.ID = workerGroup.ID;
                worker.LastName = workerGroup.LastName;
                worker.FirstMidName = workerGroup.FirstMidName;
                worker.Tag = workerGroup.Tag;
                worker.TimeOfModification = DateTime.Now;

                db.Workers.Attach(worker);
                db.Entry(worker).Property(X => X.FirstMidName).IsModified = true;
                db.Entry(worker).Property(X => X.LastName).IsModified = true;
                db.Entry(worker).Property(X => X.Tag).IsModified = true;
                db.Entry(worker).Property(X => X.TimeOfModification).IsModified = true;

                if (workerGroup.SelectedIds != null && workerGroup.SelectedIds.Count() > 0)
                {
                    var wGroups = db.GroupInstructions.Where(w => w.WorkerId == workerGroup.ID).ToList();

                    foreach (var item in workerGroup.SelectedIds)
                    {
                        if ((wGroups.Where(x => x.WorkerId.Equals(workerGroup.ID) && x.GroupId.Equals(Int32.Parse(item))).FirstOrDefault() == null) || wGroups.Count() == 0)
                        {
                            var groupInstructions = new GroupInstruction()
                            {
                                WorkerId = worker.ID,
                                TimeOfCreation = DateTime.Now,
                                TimeOfModification = DateTime.Now,
                                GroupId = Int32.Parse(item)
                            };
                            db.GroupInstructions.Add(groupInstructions);
                            db.SaveChanges();
                        }
                    }
                    foreach (var item in wGroups)
                    {
                        if (!workerGroup.SelectedIds.Contains(item.GroupId.ToString()))
                        {
                            db.GroupInstructions.Remove(item);
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    //Usuń wszystkie

                    var wGroups = db.GroupInstructions.Where(w => w.WorkerId == workerGroup.ID);

                    foreach (var g in wGroups)
                    {
                        db.GroupInstructions.Remove(g);
                    }
                    db.SaveChanges();
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(workerGroup);
        }

        // GET: Workers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Worker worker = db.Workers.Find(id);
            if (worker == null)
            {
                return HttpNotFound();
            }
            return View(worker);
        }

        // POST: Workers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Worker worker = db.Workers.Find(id);
            db.Workers.Remove(worker);
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