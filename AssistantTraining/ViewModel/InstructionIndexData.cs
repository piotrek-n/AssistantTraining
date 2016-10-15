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
        [RegularExpression(@"^\S{2}.\d{2}.\d{2}.\d{2}(.\d{2})?$", ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "NumberFormat")]
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredFiled")]
        public string Number { get; set; }

        [DisplayName("Numer szkolenia")]
        [RegularExpression(@"^\S{1,2}\d{1,3}/\d{4}$", ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "NumberFormat")]       
        //[Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredFiled")]
        public string NumberOfTraining { get; set; }

        [DisplayName("Wersja")]
        [RegularExpression(@"^\d{1,3}$", ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "NumberFormat")]
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredFiled")]
        public string Version { get; set; }

        [DisplayName("Grupy")]
        public virtual ICollection<Group> Groups { get; set; }

        public string SelectedId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredFiled")]
        public string[] SelectedIds { get; set; }

        [DisplayName("Grupy")]
        public IEnumerable<SelectListItem> Items { get; set; }

    }
}