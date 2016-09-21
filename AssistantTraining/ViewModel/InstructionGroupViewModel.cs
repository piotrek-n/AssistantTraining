using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace AssistantTraining.ViewModel
{
    public class InstructionGroupViewModel
    {
        public int ID { get; set; }

        [DisplayName("Nazwa")]
        public string Name { get; set; }
        [DisplayName("Wersja")]
        public string Version { get; set; }
    }
}