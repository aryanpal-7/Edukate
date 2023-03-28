using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Eduketa_Proj.Models
{
    public class DemoModel
    {
        public string name { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
        public string course { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? demodate { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh/:mm }", ApplyFormatInEditMode = true)]
        public TimeSpan? demotime { get; set; }
    }
}