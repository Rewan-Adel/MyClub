//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MyClubLib.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class OfferDetail
    {
        public int OfferDetailId { get; set; }
        public int OfferPriceId { get; set; }
        public int ServiceId { get; set; }
        public int ServiceLimitNumber { get; set; }
    
        public virtual OfferPrice OfferPrice { get; set; }
        public virtual OfferPrice OfferPrice1 { get; set; }
        public virtual service service { get; set; }
    }
}
