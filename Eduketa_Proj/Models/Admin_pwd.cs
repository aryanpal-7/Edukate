using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace Eduketa_Proj.Models
{
    public class Admin_pwd
    {
        [Required]
        public string email { get; set; }       
    }
    public class Admin_password
    {
        [Required]
        public string pwd { get; set; }
        [Required]
        public string cpwd { get; set; }
    }
}