using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Eduketa_Proj.Models
{
    public class AdminLogin
    {
        [Required]
        public string email { get; set; }
        [Required]

        public string password { get; set; }
    }
}