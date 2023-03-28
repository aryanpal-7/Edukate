using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eduketa_Proj.Models
{
    public class DetailsModel
    {
        public Course course { get; set; }

        public List<Course> courses { get; set; }

        public bought_course bought_course { get; set; }
    }
}