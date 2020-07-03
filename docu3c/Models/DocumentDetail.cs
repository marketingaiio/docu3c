//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace docu3c.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class DocumentDetail
    {
        public int DocumentID { get; set; }
        public int DocCustomerID { get; set; }
        public int PortfolioID { get; set; }
        public int UserID { get; set; }
        public string DocumentName { get; set; }
        public string DocumentURL { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string Institution { get; set; }
        public string JSONFileName { get; set; }
        public string FileStatus { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    
        public virtual PortfolioDetail PortfolioDetail { get; set; }
        public virtual UserDetail UserDetail { get; set; }
        public virtual CustomerDetail CustomerDetail { get; set; }
    }
}
