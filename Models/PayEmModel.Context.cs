﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PayEmFinal.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class dbPayEmEntities : DbContext
    {
        public dbPayEmEntities()
            : base("name=dbPayEmEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<BankDetail> BankDetails { get; set; }
        public virtual DbSet<CustomerBankDetailsConfidential> CustomerBankDetailsConfidentials { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<DebitCardDetail> DebitCardDetails { get; set; }
        public virtual DbSet<Fasttag> Fasttags { get; set; }
        public virtual DbSet<MobileCompany> MobileCompanies { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<UPI> UPIs { get; set; }
    }
}
