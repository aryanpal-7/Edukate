using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eduketa_Proj.Models
{
    public class CourseModel
    {
        public string name { get; set; }
        public Nullable<int> price { get; set; }
        public HttpPostedFileBase image { get; set; }
        public string description { get; set; }
    }
}