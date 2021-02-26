using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace docu3c.Models
{
    public class DocumentController
    {
        // GET: Document
        [JsonProperty("Full Name")]
        public string FullName { get; set; }
        [JsonProperty("Address")]
        public string Address { get; set; }
        [JsonProperty("AccountNo")]
        public string AccountNo { get; set; }
        [JsonProperty("SSN")]
        public string SSN { get; set; }

        [JsonProperty("DOB")]
        public string DOB { get; set; }

        [JsonProperty("SubCategory")]
        public string SubCategory { get; set; }

        [JsonProperty("Signature")]
        public string Signature { get; set; }

        [JsonProperty("ViolationofMutualFund")]
        public string ViolationofMutualFund { get; set; }
        public string IsCheckCompliance { get; set; }
    }
}