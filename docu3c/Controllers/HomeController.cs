using docu3c.Models;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace docu3c.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Dashboard dashboard = new Dashboard();
            if (Session["UserName"] != null && Session["Role"] != null && Session["UserEmailID"] != null)
            {
                string strUserEmailID = Session["UserEmailID"].ToString();
                string portfolioName = string.Empty;
                string CustomerName = string.Empty;
               
                string Documents = string.Empty;
                List<DocumentDetail> CDocumentList = new List<DocumentDetail>();
               
                int userId = 0;
                using (docu3cEntities db = new docu3cEntities())
                {
                    userId = db.UserDetails.FirstOrDefault(m => m.LoginID.Equals(strUserEmailID)).UserID;
                    CDocumentList = db.DocumentDetails.ToList();
                    //portfolioName = db.UserDetails.FirstOrDefault((m => m.LoginID.Equals(Session["UserEmailID"]))).PortfolioDetails.FirstOrDefault().PortfolioName;
                    if (User != null)
                    {
                        portfolioName = db.PortfolioDetails.FirstOrDefault(m => m.UserID.Equals(userId)).PortfolioName;
                        ViewData["NoofCustomers"] = db.CustomerDetails.Count();
                        ViewData["NoofDocumets"] = db.DocumentDetails.Count();
                        ViewData["NoofCategory"] = db.CategoryDetails.Count();
                        ViewData["NewAccountAgreement"] = db.DocumentDetails.Where(m => m.Category.Equals("1")).Count();
                        ViewData["InvestmentManagementAgreement"] = db.DocumentDetails.Where(m => m.Category.Equals("2")).Count();
                        ViewData["AutomatedFundsTransferAgreement"] = db.DocumentDetails.Where(m => m.Category.Equals("3")).Count();
                        ViewData["AssetTransferAgreements"] = db.DocumentDetails.Where(m => m.Category.Equals("4")).Count();
                        ViewData["InsuranceContractAgreements"] = db.DocumentDetails.Where(m => m.Category.Equals("5")).Count();
                        ViewData["MiscellaneousInvestments"] = db.DocumentDetails.Where(m => m.Category.Equals("6")).Count();
                        ViewData["ClientProfiles"] = db.DocumentDetails.Where(m => m.Category.Equals("7")).Count();
                        var CategoryList = CDocumentList.GroupBy(o => new

                        {
                            o.Category,
                            o.SubCategory,

                        }).Select(g => new Dashboard 
                        {

                            CategoryName = g.Key.Category,
                            SubCategoryName = g.Key.SubCategory,
                            DocumentName = g.FirstOrDefault().DocumentName,
                            iDocumentCount = g.Count(),
                        }).Where(v => v.AdvisorID == userId)


                     .ToList();
                        
                    }
                }
                dashboard.PortfolioName = portfolioName;
                return View(dashboard);
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
            DocumentUpload docUpload = new DocumentUpload();
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
                        for (int fileNum = 0; fileNum < Request.Files.Count; fileNum++)
                        {
                            string fileName = Path.GetFileName(Request.Files[fileNum].FileName);
                            if (
                             Request.Files[fileNum] != null &&
                            Request.Files[fileNum].ContentLength > 0)
                            {
                                CloudBlockBlob azureBlockBlob = storageContainer.GetBlockBlobReference(fileName);
                                azureBlockBlob.UploadFromStream(Request.Files[fileNum].InputStream);
                            }
                        }
                      //  return RedirectToAction("Index");
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
    }
}