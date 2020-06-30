using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace docu3c
{
    public class Dashboard
    {
        public string PortfolioName { get; set; }
        public int CustomerID { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public int DocCustomerID { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public string JSONIdentifier { get; set; }
        public string DocumentName { get; set; }
        public string DocumentURL { get; set; }
        public string Institution { get; set; }
        public string JSONFileName { get; set; }
        public string FileStatus { get; set; }
        public bool IsActive { get; set; }

        public int AdvisorID { get; set; }
        public System.DateTime CreatedOn { get; set; }

        public int iDocumentCount { get; set; }

       
    }
}