using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eduketa_Proj.Models
{
    public class UserDashboard
    {
        public User user { get; set; }
        public List<bought_course> bought { get; set; }

        public List<Course> course { get; set; }

       
        public List<coursepayment> coursepayments { get; set; }
    }
}