using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AssistantTraining.ViewModel
{
    public class ReportsIndexData
    {
        public string SelectedId { get; set; }
        public IEnumerable<SelectListItem> Items { get; set; }
    }
}