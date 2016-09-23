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

            viewModel.Trainings = db.Trainings
                                    .Include(t => t.Instruction)
                                    .Include(t => t.Worker)
                                    .OrderBy(t => t.Worker.LastName);

            var lst = (from w in db.Workers
                       join gi in db.GroupInstructions on w.ID equals gi.WorkerId
                       join i in db.Instructions on gi.ID equals i.GroupId 
                       //join t in db.Trainings on w.ID equals t.WorkerId
                       join t in db.Trainings on new { wID = w.ID , iID = i.ID } equals new { wID = t.WorkerId, iID = t.InstructionId }
                       into gj
                       from e in gj.DefaultIfEmpty()
                       select new { w, e }
                       );


            return View(viewModel);
        }
    }
}