using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace docu3c.Models
{
    public class Details
    {
        
        [JsonProperty("Account No.")]
        public string AccountNo { get; set; }
        [JsonProperty("Category")]
        public string Category { get; set; }

        [JsonProperty("City")]
        public string City { get; set; }

        [JsonProperty("DOB")]
        public string DOB { get; set; }

        [JsonProperty("First Name")]
        public string FirstName { get; set; }

        [JsonProperty("Last Name")]
        public string LastName { get; set; }

        [JsonProperty("Middle Name")]
        public string MiddleName { get; set; }

        [JsonProperty("Organisation")]
        public string Organisation { get; set; }

        [JsonProperty("SSN")]
        public string SSN { get; set; }

        [JsonProperty("State")]
        public string State { get; set; }

        [JsonProperty("Street")]
        public string Street { get; set; }

        [JsonProperty("Zip-code")]
        public string Zipcode { get; set; }
    }

    public class docu3cAPI
    {

        [JsonProperty("Category")]
        public string Category { get; set; }

        [JsonProperty("Organization")]
        public string Organization { get; set; }

        [JsonProperty("Selected Checkbox")]
        public IList<string> SelectedCheckbox { get; set; }

        [JsonProperty("Sub Category")]
        public string SubCategory { get; set; }

        [JsonProperty("details")]
        public Details details { get; set; }
    }

}