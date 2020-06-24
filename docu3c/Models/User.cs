using Microsoft.WindowsAzure.Storage.Table;
using System.ComponentModel.DataAnnotations;

namespace docu3c
{
    public class User : TableEntity
    {
        public User() { }
        [Key]
        public int UserId { set; get; }

        [StringLength(100)]
        [Display(Description = "First Name")]
        [Required(ErrorMessage = "Enter First Name")]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Enter valid First Name.")]
        public string FirstName { set; get; }
        [StringLength(100)]
        [Display(Description = "Last Name")]
        [Required(ErrorMessage = "Enter Last Name")]
        [RegularExpression(@"^[a-zA-Z0-9\-_\s]*$", ErrorMessage = "Enter valid Last Name.")]
        public string LastName { set; get; }

        [StringLength(100)]
        [Display(Description = "EmailID")]
        [Required(ErrorMessage = "Enter EmailID")]
        [RegularExpression(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", ErrorMessage = "Please enter a valid e-mail adress")]
        public string EmailId { set; get; }

        [StringLength(100)]
        [Display(Description = "Password")]
        [Required(ErrorMessage = "Enter the Password")]
        public string CPassword { set; get; }

        [Required(ErrorMessage = "Select any Role Type")]
        public string RoleType { set; get; }


        [StringLength(200)]
        [Display(Description = "Organization Name")]
        [Required(ErrorMessage = "Enter Organization Name")]
        public string OrganizationName { set; get; }

        [StringLength(250)]
        [Display(Description = "Address")]
        [Required(ErrorMessage = "Enter the Address")]
        public string CAddress { set; get; }

        [Display(Description = "PostalCode")]
        [RegularExpression(@"^(?!00000)[0-9]{5,5}$", ErrorMessage = "Please enter the valid Postal Code")]
        public int? PostalCode { set; get; }

        [Display(Description = "PhoneNo")]
        [RegularExpression(@"^[0-9]{5,12}$", ErrorMessage = "Please enter the valid Phone Number ")]
        public int? PhoneNo { set; get; }

        [Display(Description = "Telephone No")]
        [RegularExpression(@"^[0-9]{5,12}$", ErrorMessage = "Please enter the valid Telephone Number ")]
        public int? TeleNo { set; get; }
        public bool IsActive { set; get; }

    }
}