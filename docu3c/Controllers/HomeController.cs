using docu3c.BlobHandling;
using docu3c.Models;
using docu3c.ViewModel;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace docu3c.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
           // Dashboard dashboard = new Dashboard();
            ProfileModel ProfileModel = new ProfileModel();
            if (Session["UserName"] != null && Session["Role"] != null && Session["UserEmailID"] != null)
            {
                string strUserEmailID = Session["UserEmailID"].ToString();
                int iPortfolioID = 0;
                string CustomerName = string.Empty;
                string portfolioName = string.Empty;
                string Documents = string.Empty;
              
               
                int userId = 0;
                using (docu3cEntities db = new docu3cEntities())
                {
                    userId = db.UserDetails.FirstOrDefault(m => m.LoginID.Equals(strUserEmailID)).UserID;
                    //  CDocumentList = db.DocumentDetails.ToList();
                    //portfolioName = db.UserDetails.FirstOrDefault((m => m.LoginID.Equals(Session["UserEmailID"]))).PortfolioDetails.FirstOrDefault().PortfolioName;
                    if (User != null)
                    {


                        portfolioName = db.PortfolioDetails.FirstOrDefault(m => m.UserID.Equals(userId)).PortfolioName;
                        ViewData["PortfolioName"] = portfolioName;
                        iPortfolioID = db.PortfolioDetails.FirstOrDefault(m => m.PortfolioName.Equals(portfolioName)).PortfolioID;
                        Session["PortfolioID"] = iPortfolioID;
                        ViewData["NoofCustomers"] = db.CustomerDetails.Count();
                        ViewData["NoofDocumets"] = db.DocumentDetails.Count();
                        ViewData["NoofCategory"] = db.CategoryDetails.Count();
                        ViewData["NoOfInstitution"] = db.DocumentDetails.Distinct().GroupBy(o => new
                        {
                            o.Institution
                        }).Count();
                        ViewData["NewAccountAgreement"] = db.DocumentDetails.Where(m => m.Category.Equals("Client Agreements")).Count();
                        ViewData["InvestmentManagementAgreement"] = db.DocumentDetails.Where(m => m.Category.Equals("Investment Agreements")).Count();
                        ViewData["AutomatedFundsTransferAgreement"] = db.DocumentDetails.Where(m => m.Category.Equals("Funds Transfer Agreement")).Count();
                        ViewData["AssetTransferAgreements"] = db.DocumentDetails.Where(m => m.Category.Equals("Asset Transfer Agreements")).Count();
                        ViewData["InsuranceContractAgreements"] = db.DocumentDetails.Where(m => m.Category.Equals("Insurance Agreements")).Count();
                        ViewData["MiscellaneousInvestments"] = db.DocumentDetails.Where(m => m.Category.Equals("Miscellaneous Investments")).Count();
                        ViewData["ClientProfiles"] = db.DocumentDetails.Where(m => m.Category.Equals("Client Profile")).Count();
                        ProfileModel = new ProfileModel
                        {

                            CustomerDetails = db.CustomerDetails.ToList(),
                            PortfolioDetails = db.PortfolioDetails.ToList(),
                            DocumentDetails = db.DocumentDetails.Include("CustomerDetail").ToList(),
                            CategoryDetails = db.CategoryDetails.ToList(),
                            SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                            //ViewSubCategoryDetail = db.SubCategoryDetails.Include("CategoryDetail").GroupBy(o=>o new ).Select(c => new ViewSubCategoryDetail
                            //{

                            //    CategoryName = c.CategoryDetail.CategoryName,
                            //    Asset = c.SubCategoryName,
                            //    CustomerCount = db.DocumentDetails.FirstOrDefault(v => v.SubCategory == c.SubCategoryName).ToString().Count()

                            //})

                        };

                      

                    }
                }

             //   dashboard.PortfolioName = portfolioName;
                return View(ProfileModel);
            }
            else { return RedirectToAction("Login", "Login"); }

               
        }

       // [HttpPost]
      

        public ActionResult LogOff()
        {
            Session.Clear();
            return RedirectToAction("Login", "Login");
        }
       

        public ActionResult DocumentsUpload()
        {
            DocumentUpload docUpload = new DocumentUpload();
            if (Session["UserName"] != null && Session["Role"] != null && Session["UserEmailID"] != null && Session["PortfolioID"] !=null)
            {
                string strUserEmailID = Session["UserEmailID"].ToString();
                string portfolioName = string.Empty;
                int userId = 0;
                //int iPortfolioID = 0;
                //PortfolioDetail pd = new PortfolioDetail();
                using (docu3cEntities db = new docu3cEntities())
                {

                    userId = db.UserDetails.FirstOrDefault(m => m.LoginID.Equals(strUserEmailID)).UserID;
                    //portfolioName = db.UserDetails.FirstOrDefault((m => m.LoginID.Equals(Session["UserEmailID"]))).PortfolioDetails.FirstOrDefault().PortfolioName;
                    if (User != null)
                    {
                        portfolioName = db.PortfolioDetails.FirstOrDefault(m => m.UserID.Equals(userId)).PortfolioName;
                       
                    }
                }
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
            CloudConfigurationManager.GetSetting("azureConnectionString"));

                // Create the blob client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                CloudBlobContainer blobContainer = blobClient.GetContainerReference("uploadfiles");

                List<string> blobs = new List<string>();
                foreach (var blobItem in blobContainer.ListBlobs())
                {
                    blobs.Add(blobItem.Uri.ToString());
                }
                 
               // return View(blobs);
                docUpload.PortfolioName = portfolioName;
                return View(docUpload);
            }
            else
            { return RedirectToAction("Login", "Login"); }
            
        }
        [HttpPost]
        public ActionResult DocumentsUpload(HttpPostedFileBase[] files)
        {
            ViewBag.UploadClicked = 1;
            int iPortFolioID =Convert.ToInt32(Session["PortfolioID"]);
            BlobManager blobManagerObj = new BlobManager("uploadfiles");
            string FileAbsoluteUri;
            DocumentUpload docUpload = new DocumentUpload();
            string strUserEmailID = Session["UserEmailID"].ToString();
            int userId = 0;
            using (docu3cEntities db = new docu3cEntities())
            {
                userId = db.UserDetails.FirstOrDefault(m => m.LoginID.Equals(strUserEmailID)).UserID;
          
            //Ensure model state is valid  
            if (ModelState.IsValid)
            {   //iterating through multiple file collection   
                foreach (HttpPostedFileBase file in files)
                {

                        //Checking file is available to save.  
                        if (file != null)
                        {
                            var InputFileName = Path.GetFileName(file.FileName);
                            //  var ServerSavePath = Path.Combine(Server.MapPath("~/UploadedFiles/") + InputFileName);
                            //Save file to server folder  
                            //    file.SaveAs(ServerSavePath);
                            //assigning file uploaded status to ViewBag for showing message to user.  
                            // 


                            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("azureConnectionString"));
                            var storageClient = storageAccount.CreateCloudBlobClient();
                            var storageContainer = storageClient.GetContainerReference("uploadfiles");
                            storageContainer.CreateIfNotExists();
                            storageContainer.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                            for (int fileNum = 0; fileNum < Request.Files.Count; fileNum++)
                            {
                                string fileName = Path.GetFileName(Request.Files[fileNum].FileName);
                                if (
                                 Request.Files[fileNum] != null &&
                                Request.Files[fileNum].ContentLength > 0)
                                {
                                    CloudBlockBlob azureBlockBlob = storageContainer.GetBlockBlobReference(fileName);
                                    azureBlockBlob.Properties.ContentType = file.ContentType;
                                    azureBlockBlob.UploadFromStream(Request.Files[fileNum].InputStream);
                                    FileAbsoluteUri = azureBlockBlob.Uri.AbsoluteUri;
                                    //ViewBag.UploadStatus = files.Count().ToString() + " files uploaded successfully.";
                                    if (!string.IsNullOrEmpty(FileAbsoluteUri))
                                    {
                                        string filecontent = blobManagerObj.ReadBlob(FileAbsoluteUri);
                                        if (!string.IsNullOrEmpty(filecontent))
                                        {
                                            docu3cAPIClient d3 = new docu3cAPIClient();
                                            string fileurl = "https://docu3capp.blob.core.windows.net/uploadfiles/";
                                         
                                            var docinfo = d3.ClassifyDocument("comp", FileAbsoluteUri);
                                            //if (docinfo[0].docParseErrorMsg != null)
                                            //{
                                            //    ViewBag.ErrorMsg = "The Uploaded document is not a valid form";
                                            //}
                                          
                                            if (docinfo.Count > 0 && docinfo[0].docParseErrorMsg == null)
                                            { 
                                                string strCustomerName = string.Empty;
                                            string docURL = string.Empty;
                                            int iCustomerID = 0;
                                           

                                            if (!string.IsNullOrEmpty(docinfo[0].docURL.ToString()))
                                                docURL = docinfo[0].docURL.ToString();
                                            if (docinfo[0].docProps.ContainsKey("cust.name"))
                                                strCustomerName = docinfo[0].docProps["cust.name"].Value.ToString();
                                            var isDocCustomerAlreadyExists = db.CustomerDetails.Any(x => x.CustomerFirstName == strCustomerName);
                                            var isDocumentAlreadyExists = db.DocumentDetails.Any(x => x.DocumentURL == docURL);
                                            DocumentDetail nDocumentDetails = new DocumentDetail();
                                            //   CustomerDetail exCustomerDetails = new CustomerDetail();



                                            string strReason = string.Empty;
                                            string exSSN = string.Empty;
                                            string exAddress = string.Empty;
                                            string CustomerAddress = string.Empty;
                                            string CustomerSSN = string.Empty;
                                            string ExDocumentName = string.Empty;
                                            //  DateTime dtDOB =DateTime.Now;
                                            List<CustomerDetail> exCustomerDetails = new List<CustomerDetail>();
                                            if (!isDocumentAlreadyExists)
                                            {


                                                if (docinfo[0].docProps.ContainsKey("cust.addr"))
                                                    CustomerAddress = docinfo[0].docProps["cust.addr"].Value.ToString();
                                                if (docinfo[0].docProps.ContainsKey("cust.ssn"))
                                                    CustomerSSN = docinfo[0].docProps["cust.ssn"].Value.ToString();
                                                //if (docinfo[0].docProps.ContainsKey("cust.dob"))
                                                //{
                                                //    dtDOB =Convert.ToDateTime(docinfo[0].docProps.ContainsKey("cust.dob").ToString());

                                                //}
                                                //   dtDOB = Convert.ToDateTime(docinfo[0].docProps["cust.dob"].Value.ToString());



                                                //if(exCustomerDetails.FirstOrDefault().DOB != dtDOB)
                                                //{ strReason += string.Format("DOB is mismatch: {0}", docinfo[0].docProps.ContainsKey("cust.dob")); }

                                                if (!isDocCustomerAlreadyExists)
                                                {

                                                    CustomerDetail nCustomerDetails = new CustomerDetail();

                                                    nCustomerDetails.CustomerFirstName = strCustomerName;
                                                    nCustomerDetails.PortfolioID = iPortFolioID;
                                                    nCustomerDetails.AdvisorID = userId;
                                                    nCustomerDetails.IsActive = true;
                                                    nCustomerDetails.CreatedBy = userId.ToString();
                                                    nCustomerDetails.CreatedOn = DateTime.Now;
                                                    // if (docinfo[0].docProps.ContainsKey("cust.dob"))
                                                    //     nCustomerDetails.DOB = Convert.ToDateTime(docinfo[0].docProps["cust.dob"].Value.ToString());
                                                    nCustomerDetails.DocCustomerID = CustomerSSN;

                                                    //nCustomerDetails.DOB = Convert.ToDateTime(docinfo[0].docProps["cust.dob"].Value);
                                                    nCustomerDetails.Address = CustomerAddress;
                                                    db.CustomerDetails.Add(nCustomerDetails);
                                                    db.SaveChanges();
                                                }

                                                iCustomerID = db.CustomerDetails.FirstOrDefault(m => m.CustomerFirstName.Equals(strCustomerName)).CustomerID;

                                                exCustomerDetails = db.CustomerDetails.Where(x => x.CustomerID.Equals(iCustomerID)).ToList();
                                                //ExDocumentName = db.DocumentDetails.FirstOrDefault(m => m.CustomerID.Equals(iCustomerID)).DocumentName;
                                                if (exCustomerDetails.FirstOrDefault().Address != CustomerAddress && !string.IsNullOrEmpty(CustomerAddress))
                                                {
                                                    //  strReason += string.Format("Comparing With : {0}", ExDocumentName);

                                                    strReason += string.Format("Address is mismatch: {0} ", CustomerAddress);
                                                }
                                                if (exCustomerDetails.FirstOrDefault().DocCustomerID != CustomerSSN && !string.IsNullOrEmpty(CustomerSSN))
                                                { strReason += string.Format("SSN is mismatch: {0} ", CustomerSSN); }
                                                if (docinfo[0].docProps.ContainsKey("cust.ssn"))
                                                    nDocumentDetails.DocCustomerID = docinfo[0].docProps["cust.ssn"].Value.ToString();
                                                nDocumentDetails.PortfolioID = 1;
                                                nDocumentDetails.UserID = userId;
                                                string strJSONIdentifier = string.Empty;
                                                //    ViewBag.Message = "Comparing the Properties";
                                                //  CompareDocument CompareDocuments = db.CustomerDetails.FirstOrDefault().DocumentDetails.(m => m.PortfolioID == iPortFolioID);
                                                nDocumentDetails.CustomerID = iCustomerID;
                                                nDocumentDetails.DocumentName = System.IO.Path.GetFileName(docinfo[0].docURL.ToString());
                                                nDocumentDetails.DocumentURL = docinfo[0].docURL.ToString();
                                                nDocumentDetails.Reason = strReason;
                                                if (docinfo[0].docProps.ContainsKey("doc.type"))
                                                {
                                                    strJSONIdentifier = docinfo[0].docProps["doc.type"].Value.ToString();
                                                    strJSONIdentifier = strJSONIdentifier.Replace("_", " ");
                                                    // strJSONIdentifier = string()
                                                    nDocumentDetails.JSONFileIdentifier = strJSONIdentifier;
                                                    strJSONIdentifier = strJSONIdentifier.ToLowerInvariant();
                                                    if (db.CategoryDetails.Any(x => x.JSONIdentifier.Equals(strJSONIdentifier)))
                                                    {
                                                        nDocumentDetails.Category = db.CategoryDetails.FirstOrDefault(m => m.JSONIdentifier.Equals(strJSONIdentifier)).CategoryName;
                                                    }
                                                    if (db.SubCategoryDetails.Any(x => x.JSONIdentifier.Equals(strJSONIdentifier)))
                                                    {
                                                        int iCategoryID = 0;
                                                        string strSubCategoryName = db.SubCategoryDetails.FirstOrDefault(m => m.JSONIdentifier.Equals(strJSONIdentifier)).SubCategoryName;
                                                        nDocumentDetails.SubCategory = strSubCategoryName;
                                                        iCategoryID = db.SubCategoryDetails.FirstOrDefault(m => m.SubCategoryName.Equals(strSubCategoryName)).CategoryID;
                                                        nDocumentDetails.Category = db.CategoryDetails.FirstOrDefault(m => m.CategoryID.Equals(iCategoryID)).CategoryName;
                                                    }


                                                }
                                                if (docinfo[0].docProps.ContainsKey("org.name"))
                                                    nDocumentDetails.Institution = docinfo[0].docProps["org.name"].Value.ToString();
                                                if (string.IsNullOrEmpty(strReason))
                                                {
                                                    nDocumentDetails.FileStatus = "Green";
                                                }
                                                else { nDocumentDetails.FileStatus = "Red"; }
                                                nDocumentDetails.IsActive = true;
                                                nDocumentDetails.CreatedBy = userId.ToString();
                                                nDocumentDetails.CreatedOn = DateTime.Now;
                                                db.DocumentDetails.Add(nDocumentDetails);

                                                db.SaveChanges();
                                            }
                                                else
                                                {
                                                    ModelState.AddModelError("UploadedDocuments", "Uploaded documents are not able to extract the values ");

                                                }
                                            }
                                            else {
                                                ModelState.AddModelError("UploadedDocuments", "Uploaded documents are not able to extract the values ");
                                              
                                            }
                                        }

                                    }
                                 //   else { ModelState.AddModelError("UploadedDocuments", "Uploaded documents are not able to extract the values "); }
                                }
                                
                              
                            }
                            return RedirectToAction("DocumentDetails");
                        }
                        }

                }
            }
            return View(docUpload);
        }



       

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult DocumentDetails()
        {
            ProfileModel ProfileModel = new ProfileModel();
            if (Session["UserName"] != null && Session["Role"] != null && Session["UserEmailID"] != null)
            {
                string strUserEmailID = Session["UserEmailID"].ToString();
                int userId = 0;
                using (docu3cEntities db = new docu3cEntities())
                {
                    userId = db.UserDetails.FirstOrDefault(m => m.LoginID.Equals(strUserEmailID)).UserID;
                    //  CDocumentList = db.DocumentDetails.ToList();
                    //portfolioName = db.UserDetails.FirstOrDefault((m => m.LoginID.Equals(Session["UserEmailID"]))).PortfolioDetails.FirstOrDefault().PortfolioName;
                    if (User != null)
                    {
                        ViewData["PortfolioName"] = db.PortfolioDetails.FirstOrDefault(m => m.UserID.Equals(userId)).PortfolioName;
                        ViewData["NoofCustomers"] = db.CustomerDetails.Count();
                        ViewData["NoofDocumets"] = db.DocumentDetails.Count();
                        ViewData["NoofCategory"] = db.CategoryDetails.Count();
                        ViewData["NoOfInstitution"] = db.DocumentDetails.Distinct().GroupBy(o=>new
                        {
                            o.Institution
                        }).Count();
                        ViewData["NewAccountAgreement"] = db.DocumentDetails.Where(m => m.Category.Equals("1")).Count();
                        ViewData["InvestmentManagementAgreement"] = db.DocumentDetails.Where(m => m.Category.Equals("2")).Count();
                        ViewData["AutomatedFundsTransferAgreement"] = db.DocumentDetails.Where(m => m.Category.Equals("3")).Count();
                        ViewData["AssetTransferAgreements"] = db.DocumentDetails.Where(m => m.Category.Equals("4")).Count();
                        ViewData["InsuranceContractAgreements"] = db.DocumentDetails.Where(m => m.Category.Equals("5")).Count();
                        ViewData["MiscellaneousInvestments"] = db.DocumentDetails.Where(m => m.Category.Equals("6")).Count();
                        ViewData["ClientProfiles"] = db.DocumentDetails.Where(m => m.Category.Equals("7")).Count();

                        ProfileModel = new ProfileModel
                        {

                            CustomerDetails = db.CustomerDetails.ToList(),
                            PortfolioDetails = db.PortfolioDetails.ToList(),
                            DocumentDetails = db.DocumentDetails.Include("CustomerDetail").ToList(),
                            CategoryDetails = db.CategoryDetails.ToList(),
                            SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                        };
                    }
                }
                return View(ProfileModel);
            }
            else { return RedirectToAction("Login", "Login"); }
           
        }
        public ActionResult CustomerDetails()
        {
            ProfileModel ProfileModel = new ProfileModel();
            if (Session["UserName"] != null && Session["Role"] != null && Session["UserEmailID"] != null)
            {
                string strUserEmailID = Session["UserEmailID"].ToString();
                int userId = 0;
                using (docu3cEntities db = new docu3cEntities())
                {
                    userId = db.UserDetails.FirstOrDefault(m => m.LoginID.Equals(strUserEmailID)).UserID;
                    //  CDocumentList = db.DocumentDetails.ToList();
                    //portfolioName = db.UserDetails.FirstOrDefault((m => m.LoginID.Equals(Session["UserEmailID"]))).PortfolioDetails.FirstOrDefault().PortfolioName;
                    if (User != null)
                    {
                        ViewData["PortfolioName"] = db.PortfolioDetails.FirstOrDefault(m => m.UserID.Equals(userId)).PortfolioName;
                        ViewData["NoofCustomers"] = db.CustomerDetails.Count();
                        ViewData["NoofDocumets"] = db.DocumentDetails.Count();
                        ViewData["NoofCategory"] = db.CategoryDetails.Count();
                        ViewData["NoOfInstitution"] = db.DocumentDetails.Distinct().GroupBy(o => new
                        {
                            o.Institution
                        }).Count();
                        ViewData["NewAccountAgreement"] = db.DocumentDetails.Where(m => m.Category.Equals("1")).Count();
                        ViewData["InvestmentManagementAgreement"] = db.DocumentDetails.Where(m => m.Category.Equals("2")).Count();
                        ViewData["AutomatedFundsTransferAgreement"] = db.DocumentDetails.Where(m => m.Category.Equals("3")).Count();
                        ViewData["AssetTransferAgreements"] = db.DocumentDetails.Where(m => m.Category.Equals("4")).Count();
                        ViewData["InsuranceContractAgreements"] = db.DocumentDetails.Where(m => m.Category.Equals("5")).Count();
                        ViewData["MiscellaneousInvestments"] = db.DocumentDetails.Where(m => m.Category.Equals("6")).Count();
                        ViewData["ClientProfiles"] = db.DocumentDetails.Where(m => m.Category.Equals("7")).Count();

                        ProfileModel = new ProfileModel
                        {

                            CustomerDetails = db.CustomerDetails.ToList(),
                            PortfolioDetails = db.PortfolioDetails.ToList(),
                            DocumentDetails = db.DocumentDetails.Include("CustomerDetail").ToList(),
                            CategoryDetails = db.CategoryDetails.ToList(),
                            SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                        };
                    }
                }
                return View(ProfileModel);
            }
            else { return RedirectToAction("Login", "Login"); }
           
        }

        public ActionResult InstitutionDetails()
        {
            ProfileModel ProfileModel = new ProfileModel();
            if (Session["UserName"] != null && Session["Role"] != null && Session["UserEmailID"] != null)
            {
                string strUserEmailID = Session["UserEmailID"].ToString();
                int userId = 0;
                using (docu3cEntities db = new docu3cEntities())
                {
                    userId = db.UserDetails.FirstOrDefault(m => m.LoginID.Equals(strUserEmailID)).UserID;
                    //  CDocumentList = db.DocumentDetails.ToList();
                    //portfolioName = db.UserDetails.FirstOrDefault((m => m.LoginID.Equals(Session["UserEmailID"]))).PortfolioDetails.FirstOrDefault().PortfolioName;
                    if (User != null)
                    {
                        ViewData["PortfolioName"] = db.PortfolioDetails.FirstOrDefault(m => m.UserID.Equals(userId)).PortfolioName;
                        ViewData["NoofCustomers"] = db.CustomerDetails.Count();
                        ViewData["NoofDocumets"] = db.DocumentDetails.Count();
                        ViewData["NoofCategory"] = db.CategoryDetails.Count();
                        ViewData["NoOfInstitution"] = db.DocumentDetails.Distinct().GroupBy(o => new
                        {
                            o.Institution
                        }).Count();
                        ViewData["NewAccountAgreement"] = db.DocumentDetails.Where(m => m.Category.Equals("1")).Count();
                        ViewData["InvestmentManagementAgreement"] = db.DocumentDetails.Where(m => m.Category.Equals("2")).Count();
                        ViewData["AutomatedFundsTransferAgreement"] = db.DocumentDetails.Where(m => m.Category.Equals("3")).Count();
                        ViewData["AssetTransferAgreements"] = db.DocumentDetails.Where(m => m.Category.Equals("4")).Count();
                        ViewData["InsuranceContractAgreements"] = db.DocumentDetails.Where(m => m.Category.Equals("5")).Count();
                        ViewData["MiscellaneousInvestments"] = db.DocumentDetails.Where(m => m.Category.Equals("6")).Count();
                        ViewData["ClientProfiles"] = db.DocumentDetails.Where(m => m.Category.Equals("7")).Count();

                        ProfileModel = new ProfileModel
                        {

                            CustomerDetails = db.CustomerDetails.ToList(),
                            PortfolioDetails = db.PortfolioDetails.ToList(),
                            DocumentDetails = db.DocumentDetails.ToList(),
                          
                            CategoryDetails = db.CategoryDetails.ToList(),
                            SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                        };
                    }
                }
                return View(ProfileModel);
            }
            else { return RedirectToAction("Login", "Login"); }

        }
    }
}