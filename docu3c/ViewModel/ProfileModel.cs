using docu3c.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace docu3c.ViewModel
{
    public class ProfileModel
    {
        public IEnumerable<CustomerDetail>  CustomerDetails { set; get; }
        public IEnumerable<PortfolioDetail> PortfolioDetails { set; get; }

        public IEnumerable<DocumentDetail> DocumentDetails { set; get; }
        public IEnumerable<CategoryDetail> CategoryDetails { set; get; }

        public IEnumerable<SubCategoryDetail> SubCategoryDetails { set; get; }
        public IEnumerable<ViewSubCategoryDetail> ViewSubCategoryDetail { set; get; }

        public IEnumerable<UserDetail> UserDetails { set; get; }


    }

    public class ViewSubCategoryDetail
    {
        public string CategoryName { set; get; }
        public string Asset { set; get; }
        public int CustomerCount { set; get; }
    }
}