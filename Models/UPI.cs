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
    
    public partial class UPI
    {
        public UPI() { }
        public UPI(string id, string pin, string phno)
        {
            UPIID = id;
            UPIPin = pin;
            PhoneNumber = phno;
        }
        public string UPIID { get; set; }
        public string UPIPin { get; set; }
        public string PhoneNumber { get; set; }
    
        public virtual Customer Customer { get; set; }
    }
}
