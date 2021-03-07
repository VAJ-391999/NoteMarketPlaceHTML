//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NoteMarketPlace.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class NoteReview
    {
        public int ID { get; set; }
        public int NoteID { get; set; }
        public int AgainstDownloadsID { get; set; }
        public decimal Ratings { get; set; }
        public string Feedback { get; set; }
        public Nullable<bool> Inappropriate { get; set; }
        public int BuyersID { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    
        public virtual DownloadedNote DownloadedNote { get; set; }
        public virtual Note Note { get; set; }
        public virtual User User { get; set; }
    }
}