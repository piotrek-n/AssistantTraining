using AssistantTraining.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AssistantTraining.ViewModel
{
    public class InstructionIndexData
    {
        public int ID { get; set; }

        [DisplayName("Nazwa")]
        public string Name { get; set; }

        [DisplayName("Numer")]
        public string Number { get; set; }

        [DisplayName("Wersja")]
        public string Version { get; set; }

        [DisplayName("Grupy")]
        public virtual ICollection<Group> Groups { get; set; }

        public string SelectedId { get; set; }

        public string[] SelectedIds { get; set; }

        [DisplayName("Grupy")]
        public IEnumerable<SelectListItem> Items { get; set; }

    }
}