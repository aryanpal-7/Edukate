using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Eduketa_Proj.Models
{
    public class ContactModel
    {
        [Required(ErrorMessage ="Username is Required.")]
        
        public string UserName { get; set; }
        [Required(ErrorMessage ="Email is Required.")]
        public string email { get; set; }
        [Required(ErrorMessage ="Enter Your Subject.")]
        public string Subject { get; set; }
        [Required(ErrorMessage ="Message is Required.")]
        public string Message { get; set; }
        public Nullable<int> userid { get; set; }

    }
}