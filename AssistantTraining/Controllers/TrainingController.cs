using AssistantTraining.DAL;
using AssistantTraining.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace AssistantTraining.Controllers
{
    [Authorize]
    public class TrainingController : Controller
    {
        /* Staff Training*/
        /*z jakich instrukcji pracownik powinien być przeszkolony i czy te szkolenia się odbyły*/
        /*
         * W1
         *  I1 Przeszkolony: Tak/Nie T.Date
         *  I2 Przeszkolony: Tak/Nie T.Date
         * W2
         *  I1 Przeszkolony: Tak/Nie T.Date
         *  I2 Przeszkolony: Tak/Nie T.Date
         *  
                Select * from [dbo].[Workers] w
                LEFT JOIN [dbo].[GroupInstructions] gi ON w.ID = gi.WorkerID
                LEFT JOIN [dbo].[Instructions] i ON gi.GroupID = i.GroupID
                LEFT JOIN [dbo].[Trainings] t ON t.WorkerId = w.ID AND t.[InstructionId] = i.ID  
                order BY w.LastName, w.FirstMidName
         */

        /* Training */
        /*śledzenie instrukcji – jacy pracownicy są podpięci do tych instrukcji i którzy wymagają przeszkolenia*/
        /*
         * I1
         *  W1 Przeszkolony: Tak/Nie
         *  W2 Przeszkolony: Tak/Nie
         * I12
         *  W1 Przeszkolony: Tak/Nie
         *  W2 Przeszkolony: Tak/Nie
         */

        private AssistantTrainingContext db = new AssistantTrainingContext();

        public ActionResult Index(int? id)
        {
            var viewModel = new TrainingIndexData();



            var lst = (from w in db.Workers
                       join gw in db.GroupWorkers on w.ID equals gw.WorkerId
                       join ig in db.InstructionGroups on new { GroupId = gw.GroupId } equals new { GroupId = Convert.ToInt32(ig.GroupId) }
                       join i in db.Instructions on ig.InstructionId equals i.ID
                       join t in db.Trainings on new { wID = w.ID , iID = i.ID } equals new { wID = t.WorkerId, iID = t.InstructionId }
                       into gj
                       from e in gj.DefaultIfEmpty()
                       select new TrainingItemIndexData{ WorkId=w.ID, Worker =w, Instruction =i, Training =e}
                       //select new {w, i= (new InstructionExt(w.ID,i)),e = (new TrainingExt{ WorkerID = w.ID }) }
                       ).OrderBy(x=>x.Worker.LastName).ToList();
            viewModel.items = lst;

            return View(viewModel);
        }
    }
}