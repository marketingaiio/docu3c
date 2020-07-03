
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace docu3c
{
    public class DocumentUpload
    {
        [Required(ErrorMessage = "Please select file.")]
        [Display(Name = "Browse File")]
        public HttpPostedFileBase[] files { get; set; }
        public string PortfolioName { get; set; }

      

    }
}