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

    public partial class CustomerBankDetailsConfidential
    {
        [Display(Name = "Account Number")]
        [RegularExpression(@"^([0-9]{8,17})$", ErrorMessage = "Please enter valid Account Number.")]
        public string AccountNumber { get; set; }
        [Display(Name = "Customer Name")]
        public string NameOfTheCustomer { get; set; }
        public string Address { get; set; }

        [Display(Name = "Phone Number")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Phone Number.")]
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string BankName { get; set; }

        [RegularExpression(@"^[A-Z]{4}0[A-Z0-9]{6}$", ErrorMessage = "Invalid IFSC Code.")]
        public string IFSC { get; set; }
        public string CardNumber { get; set; }
        public System.DateTime ValidUpto { get; set; }
        public string CVV { get; set; }
        public string Balance { get; set; }
    }
}
