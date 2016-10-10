using AssistantTraining.App_GlobalResources;
using AssistantTraining.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AssistantTraining.ViewModel
{
    public class InstructionIndexData
    {
        public int ID { get; set; }

        [DisplayName("Nazwa")]
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredFiled")]
        public string Name { get; set; }

        [DisplayName("Numer")]
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredFiled")]
        public string Number { get; set; }

        [DisplayName("Numer szkolenia")]
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredFiled")]
        public string NumberOfTraining { get; set; }

        [DisplayName("Wersja")]
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredFiled")]
        public string Version { get; set; }

        [DisplayName("Grupy")]
        public virtual ICollection<Group> Groups { get; set; }

        public string SelectedId { get; set; }

        public string[] SelectedIds { get; set; }

        [DisplayName("Grupy")]
        public IEnumerable<SelectListItem> Items { get; set; }

    }
}