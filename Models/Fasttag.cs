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

    public partial class Fasttag
    {
        public Fasttag()
        {

        }

        public Fasttag(string fid, string vehicleno, string phno, string wallet)
        {
            FasttagId = fid;
            CarNumber = vehicleno;
            PhoneNumber = phno;
            WalletAmount = wallet;
        }

        public string FasttagId { get; set; }

        [Required(ErrorMessage = "Please enter vehicle number")]
        [Display(Name = "Vehicle Amount")]
        [RegularExpression(@"^[A-Z]{2}[0-9]{2}[A-Z]{2}[0-9]{4}$", ErrorMessage = "Invalid Vehicle Number.")]
        public string CarNumber { get; set; }

        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please enter amount.")]
        [Display(Name = "Wallet Amount")]
        [RegularExpression(@"^([0-9]*)$", ErrorMessage = "Invalid amount.")]
        public string WalletAmount { get; set; }
    }
}
