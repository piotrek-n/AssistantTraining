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
    [Authorize]
    public class InstructionsController : Controller
    {
        private AssistantTrainingContext db = new AssistantTrainingContext();

        // GET: Instructions
        public ActionResult Index()
        {
            var allInstructions = db.Instructions.ToList();
            var workerRepository = new WorkerRepository();
            var groups = workerRepository.GetAllGroups();

            List<InstructionIndexData> lstInstructionGroups = new List<InstructionIndexData>();

            foreach (var item in allInstructions)
            {
                var instructioGroup = new InstructionIndexData();

                instructioGroup.ID = item.ID;
                instructioGroup.Name = item.Name;
                instructioGroup.Version = item.Version;
                instructioGroup.SelectedId = item.GroupId.ToString();
                instructioGroup.Items = groups.Select(x => new SelectListItem
                {
                    Value = x.ID.ToString(),
                    Text = x.GroupName
                });

                lstInstructionGroups.Add(instructioGroup);
            }

            return View(lstInstructionGroups);
        }

        // GET: Instructions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var instruction = db.Instructions.Find(id);

            if (instruction == null)
            {
                return HttpNotFound();
            }

            InstructionIndexData instructionGroupViewModel = new InstructionIndexData();
            var workerRepository = new WorkerRepository();
            var groups = workerRepository.GetAllGroups();

            instructionGroupViewModel.Name = instruction.Name;
            instructionGroupViewModel.Version = instruction.Version;
            instructionGroupViewModel.SelectedId = instruction.GroupId.ToString();

            instructionGroupViewModel.Items = groups.Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.GroupName
            });
            if (instructionGroupViewModel == null)
            {
                return HttpNotFound();
            }
            return View(instructionGroupViewModel);
        }

        // GET: Instructions/Create
        public ActionResult Create()
        {
            var instructionGroup = new InstructionIndexData();
            var workerRepository = new WorkerRepository();
            var groups = workerRepository.GetAllGroups();

            instructionGroup.Items = groups.Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.GroupName
            });

            return View(instructionGroup);
        }

        // POST: Instructions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InstructionIndexData instructionGroupViewModel)
        {
            if (ModelState.IsValid)
            {
                //db.InstructionGroupViewModels.Add(instructionGroupViewModel);
                Instruction instruction = new Instruction();
                instruction.Name = instructionGroupViewModel.Name;
                instruction.Version = instructionGroupViewModel.Version;
                instruction.TimeOfCreation = DateTime.Now;
                instruction.TimeOfModification = DateTime.Now;
                instruction.GroupId = Int32.Parse(instructionGroupViewModel.SelectedId);
                db.Instructions.Add(instruction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(instructionGroupViewModel);
        }

        // GET: Instructions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var instruction = db.Instructions.Find(id);

            if (instruction == null)
            {
                return HttpNotFound();
            }

            InstructionIndexData instructionGroupViewModel = new InstructionIndexData();
            var workerRepository = new WorkerRepository();
            var groups = workerRepository.GetAllGroups();

            instructionGroupViewModel.Name = instruction.Name;
            instructionGroupViewModel.Version = instruction.Version;

            instructionGroupViewModel.Items = groups.Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.GroupName
            });

            return View(instructionGroupViewModel);
        }

        // POST: Instructions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(InstructionIndexData instructionGroupViewModel)
        {
            if (ModelState.IsValid)
            {
                Instruction instruction = new Instruction();
                instruction.ID = instructionGroupViewModel.ID;
                instruction.TimeOfModification = DateTime.Now;
                instruction.Name = instructionGroupViewModel.Name;
                instruction.Version = instructionGroupViewModel.Version;
                instruction.GroupId = Int32.Parse(instructionGroupViewModel.SelectedId);

                db.Instructions.Attach(instruction);
                db.Entry(instruction).Property(X => X.Name).IsModified = true;
                db.Entry(instruction).Property(X => X.Version).IsModified = true;
                db.Entry(instruction).Property(X => X.GroupId).IsModified = true;
                db.Entry(instruction).Property(X => X.Tag).IsModified = true;
                db.Entry(instruction).Property(X => X.TimeOfModification).IsModified = true;

                db.SaveChanges();
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(instructionGroupViewModel);
        }

        // GET: Instructions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var instruction = db.Instructions.Find(id);

            if (instruction == null)
            {
                return HttpNotFound();
            }

            InstructionIndexData instructionGroupViewModel = new InstructionIndexData();
            var workerRepository = new WorkerRepository();
            var groups = workerRepository.GetAllGroups();

            instructionGroupViewModel.Name = instruction.Name;
            instructionGroupViewModel.Version = instruction.Version;
            instructionGroupViewModel.SelectedId = instruction.GroupId.ToString();

            instructionGroupViewModel.Items = groups.Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.GroupName
            });
            if (instructionGroupViewModel == null)
            {
                return HttpNotFound();
            }
            return View(instructionGroupViewModel);
        }

        // POST: Instructions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var instruction = db.Instructions.Find(id);
            db.Instructions.Remove(instruction);
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