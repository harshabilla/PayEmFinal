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
    
    public partial class Transaction
    {
        public Transaction()
        { }

        public Transaction(string fromuser, string touser, string tid, string amount, string fromcard, string tocard, DateTime ttime, string ttype)
        {
            FromUser = fromuser;
            ToUser = touser;
            TransactionId = tid;
            Amount = amount;
            FromCard = fromcard;
            ToCard = tocard;
            TransactionTime = ttime;
            TransactionType = ttype;
        }
        public string FromUser { get; set; }
        public string ToUser { get; set; }
        public string TransactionId { get; set; }
        public string Amount { get; set; }
        public string FromCard { get; set; }
        public string ToCard { get; set; }
        public System.DateTime TransactionTime { get; set; }
        public string TransactionType { get; set; }
    }
}
