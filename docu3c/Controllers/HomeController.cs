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
              //  string portfolioName = string.Empty;
                string CustomerName = string.Empty;
               
                string Documents = string.Empty;
              
               
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
                        ViewData["NoOfInstitution"] = db.DocumentDetails.Distinct().Count();
                        ViewData["NewAccountAgreement"] = db.DocumentDetails.Where(m => m.Category.Equals("New Account Agreements")).Count();
                        ViewData["InvestmentManagementAgreement"] = db.DocumentDetails.Where(m => m.Category.Equals("Investment Management Agreements")).Count();
                        ViewData["AutomatedFundsTransferAgreement"] = db.DocumentDetails.Where(m => m.Category.Equals("Automated Funds Transfer Agreement")).Count();
                        ViewData["AssetTransferAgreements"] = db.DocumentDetails.Where(m => m.Category.Equals("Asset Transfer Agreements")).Count();
                        ViewData["InsuranceContractAgreements"] = db.DocumentDetails.Where(m => m.Category.Equals("Insurance Contract Agreements")).Count();
                        ViewData["MiscellaneousInvestments"] = db.DocumentDetails.Where(m => m.Category.Equals("Miscellaneous Investments")).Count();
                        ViewData["ClientProfiles"] = db.DocumentDetails.Where(m => m.Category.Equals("Client Profiles")).Count();
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

        public ActionResult LogOff()
        {
            Session.Clear();
            return RedirectToAction("Login", "Login");
        }
       

        public ActionResult DocumentsUpload()
        {
            DocumentUpload docUpload = new DocumentUpload();
            if (Session["UserName"] != null && Session["Role"] != null && Session["UserEmailID"] != null)
            {
                string strUserEmailID = Session["UserEmailID"].ToString();
                string portfolioName = string.Empty;
                int userId = 0;
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
                            //    ViewBag.UploadStatus = files.Count().ToString() + " files uploaded successfully.";


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
                                    if (!string.IsNullOrEmpty(FileAbsoluteUri))
                                    {
                                        string filecontent = blobManagerObj.ReadBlob(FileAbsoluteUri);
                                        if (!string.IsNullOrEmpty(filecontent))
                                        {
                                            docu3cAPIClient d3 = new docu3cAPIClient();
                                            string fileurl = "https://docu3capp.blob.core.windows.net/uploadfiles/";
                                            //var url = HttpContext.Request.UserHostName.ToString();
                                            // string classifytxtresult = ClassifyText(filecontent);
                                            var docinfo = d3.ClassifyDocument("comp", FileAbsoluteUri);
                                            int docCusID = Convert.ToInt32(docinfo[0].docProps["cust.ssn"].Value);
                                            string docURL = docinfo[0].docURL.ToString();
                                            var isDocCustomerIDAlreadyExists = db.CustomerDetails.Any(x => x.DocCustomerID == docCusID);
                                            var isDocumentAlreadyExists = db.DocumentDetails.Any(x => x.DocumentURL == docURL);
                                            if (!isDocumentAlreadyExists)
                                            {
                                                if (!isDocCustomerIDAlreadyExists)
                                                {
                                                    CustomerDetail nCustomerDetails = new CustomerDetail();
                                                    nCustomerDetails.DocCustomerID = docCusID;
                                                    nCustomerDetails.CustomerFirstName = docinfo[0].docProps["cust.name"].Value.ToString();
                                                    nCustomerDetails.PortfolioID = 1;
                                                    nCustomerDetails.AdvisorID = userId;
                                                    nCustomerDetails.IsActive = true;
                                                    nCustomerDetails.CreatedBy = userId.ToString();
                                                    nCustomerDetails.CreatedOn = DateTime.Now;
                                                    //     nCustomerDetails.DOB =Convert.ToDateTime(docinfo[0].docProps["cust.dob"].Value.ToString());
                                                    //  string cdAddress = docinfo[0].docProps["cust.addr"].Value.ToString();
                                                    //if (!string.IsNullOrEmpty(cdAddress))
                                                    //{
                                                    //    nCustomerDetails.Address = cdAddress;
                                                    //}
                                                   db.CustomerDetails.Add(nCustomerDetails);
                                                    db.SaveChanges();

                                                }
                                                DocumentDetail nDocumentDetails = new DocumentDetail();
                                                nDocumentDetails.DocCustomerID = docCusID;
                                                nDocumentDetails.PortfolioID = 1;
                                                nDocumentDetails.UserID = userId;
                                                nDocumentDetails.DocumentName = System.IO.Path.GetFileName(docinfo[0].docURL.ToString());
                                                nDocumentDetails.DocumentURL = docURL;
                                                nDocumentDetails.Category = docinfo[0].docProps["doc.type"].Value.ToString();
                                                nDocumentDetails.FileStatus = "Green";
                                                nDocumentDetails.Institution = docinfo[0].docProps["org.name"].Value.ToString();
                                                nDocumentDetails.IsActive = true;
                                                nDocumentDetails.CreatedBy = userId.ToString();
                                                nDocumentDetails.CreatedOn = DateTime.Now;
                                                db.DocumentDetails.Add(nDocumentDetails);
                                               
                                                db.SaveChanges();
                                            }
                                        } 
                                        
                                    }
                                }

                               return RedirectToAction("DocumentDetails");
                            }
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
                        ViewData["NoOfInstitution"] = db.DocumentDetails.Distinct().Count();
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
                        ViewData["NoOfInstitution"] = db.DocumentDetails.Distinct().Count();
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
                        ViewData["NoOfInstitution"] = db.DocumentDetails.Distinct().Count();
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
    }
}