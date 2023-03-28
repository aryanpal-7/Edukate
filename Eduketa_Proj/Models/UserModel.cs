using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Eduketa_Proj.Models
{
    public class UserModel
    {
        [Required]
        public string name { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        [Compare("password",ErrorMessage ="Password Doesn't Match")]
        public string cpassword { get; set; }
    }
}