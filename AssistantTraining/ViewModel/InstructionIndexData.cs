using AssistantTraining.App_GlobalResources;
using AssistantTraining.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;

namespace AssistantTraining.ViewModel
{
    public class InstructionIndexData
    {
        public int ID { get; set; }

        [DisplayName("Nazwa")]
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredFiled")]
        public string Name { get; set; }

        [DisplayName("Numer")]
        [RegularExpression(@"^[A-Z]{2}.\d{2}.\d{2}(.\d{2,3})?(.\d{2,3})?$", ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "NumberFormat")]
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredFiled")]
        public string Number { get; set; }

        [DisplayName("Numer szkolenia")]
        [RegularExpression(@"^\S{1,2}\d{1,3}/\d{4}$", ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "NumberFormat")]       
        //[Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredFiled")]
        public string NumberOfTraining { get; set; }

        [DisplayName("Wersja")]
        [RegularExpression(@"^\d{1,3}$", ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "NumberFormat")]
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredFiled")]
        public int Version { get; set; }

        [DisplayName("Grupy")]
        public virtual ICollection<Group> Groups { get; set; }

        public string SelectedId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredFiled")]
        public string[] SelectedIds { get; set; }

        //We want to replace it by ItemsList
        [DisplayName("Grupy")]
        public IEnumerable<SelectListItem> Items { get; set; }

        public string UserName { get; set; }

        public string TimeOfCreation { get; set; }

        public int RowNo { get; set; }

        public List<IInputGroupItem> ItemsList;
        
        //[Required]
        public string[] CheckBoxGroupValue { get; set; }

    }
}