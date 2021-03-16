//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class BankDetail
    {
        [Required(ErrorMessage = "Please enter your Account Number.")]
        [Display(Name = "Account Number")]
        [RegularExpression(@"^([0-9]{8,17})$", ErrorMessage = "Invalid Account Number.")]
        public string AccountNumber { get; set; }

        [NotMapped]
        [Compare("AccountNumber", ErrorMessage = "Cannot match account numbers.")]
        public string ConfirmAccountNumber { get; set; }

        [Display(Name = "Name of Bank")]
        public string BankName { get; set; }

        [Required(ErrorMessage = "Please enter your IFSC.")]
        [Display(Name = "IFSC Code")]
        [RegularExpression(@"^[A-Z]{4}0[A-Z0-9]{6}$", ErrorMessage = "Invalid IFSC Code.")]
        public string IFSC { get; set; }

        [Required(ErrorMessage = "Please enter your Phone Number.")]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Phone Number.")]
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        [NotMapped]
        [Display(Name = "Amount")]
        public double Amount { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
