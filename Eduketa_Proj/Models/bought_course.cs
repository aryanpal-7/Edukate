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
    
    public partial class bought_course
    {
        public int id { get; set; }
        public Nullable<int> userid { get; set; }
        public Nullable<int> course_id { get; set; }
        public Nullable<System.DateTime> Purchased_Date { get; set; }
        public Nullable<int> price { get; set; }
    }
}
