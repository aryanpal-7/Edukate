//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Eduketa_Proj.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Contact
    {
        public int id { get; set; }
        public string UserName { get; set; }
        public string email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public Nullable<int> userid { get; set; }
        public string Resp { get; set; }
        public Nullable<int> Sellerid { get; set; }
    }
}
