﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class eduketaEntities1 : DbContext
    {
        public eduketaEntities1()
            : base("name=eduketaEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<bought_course> bought_course { get; set; }
        public virtual DbSet<coursepayment> coursepayments { get; set; }
        public virtual DbSet<forget> forgets { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<emailOTP> emailOTPs { get; set; }
        public virtual DbSet<Seller> Sellers { get; set; }
        public virtual DbSet<seller_verify> seller_verify { get; set; }
    }
}
