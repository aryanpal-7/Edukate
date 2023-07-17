using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Eduketa_Proj.Models
{
    public class UserModel
    {
        [Required(ErrorMessage ="Enter Your Name")]
        [MinLength(2,ErrorMessage ="Length of Name should be more than 1 character.")]
        [MaxLength(100,ErrorMessage ="Length of Name should not be more than 100 characters.")]
        public string name { get; set; }
        [Required(ErrorMessage ="Enter Your Email")]
        public string email { get; set; }
        [Required(ErrorMessage ="Enter Your Password")]
        [MinLength(8,ErrorMessage ="Password length should be atleast 8 characters")]
        [MaxLength(18,ErrorMessage ="Password length should not exceed 18 characters")]
        public string password { get; set; }
        [Required(ErrorMessage ="Enter Your Password Again")]
        [Compare("password",ErrorMessage ="Password Doesn't Match")]
        public string cpassword { get; set; }
    }
}