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

        public List<String> image { get; set; }

        public List<String> coursename { get; set; }
        public List<String> Description { get; set; }
        public List<int> Price { get; set; }

        public List<String> orderid { get; set; }
    }
}