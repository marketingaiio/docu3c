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
    
    public partial class DocumentIdentifier
    {
        public int CIdentifierID { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string JSONIdentifier { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    }
}
