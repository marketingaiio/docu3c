using docu3c.BlobHandling;
using docu3c.Models;
using docu3c.ViewModel;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
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
            //Session["dPortFolioID"] = 0;
            // Dashboard dashboard = new Dashboard();
            ProfileModel ProfileModel = new ProfileModel();
            if (Session["UserName"] != null && Session["Role"] != null && Session["UserEmailID"] != null)
            {
                string strUserEmailID = Session["UserEmailID"].ToString();
                // int iPortfolioID = 0;
                string CustomerName = string.Empty;
                string portfolioName = string.Empty;
                string Documents = string.Empty;
                int SessionPortFolioID;
                if (Session["dPortFolioID"] != null)
                {
                    SessionPortFolioID = Convert.ToInt32(Session["dPortFolioID"].ToString());
                }
                else { SessionPortFolioID = 0; }



                // int userId = 0;
                using (docu3cEntities db = new docu3cEntities())
                {
                    //  userId = db.UserDetails.FirstOrDefault(m => m.LoginID.Equals(strUserEmailID)).UserID;

                    if (User != null)
                    {




                        if (SessionPortFolioID == 0)
                        {
                            ViewData["NoofCustomers"] = db.CustomerDetails.Count();
                            ViewData["NoofDocumets"] = db.DocumentDetails.Count();
                           
                                

                            // g.Select(s => s.p.PID).Where(z => z != null).Distinct().Count()
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


                            };
                            //ViewData["NoofCategory"] = ProfileModel.DocumentDetails.Where(o => o.Category != null).Distinct().GroupBy(o => new
                            //{
                            //    o.Category 
                            //}).Count();

                            ViewData["NoofCategory"] = ProfileModel.DocumentDetails.Select(o => o.Category).Distinct().Count();

                        }
                        else
                        {
                            ViewData["NoofCustomers"] = db.CustomerDetails.Where(x => x.PortfolioID == SessionPortFolioID).Count();
                            ViewData["NoofDocumets"] = db.DocumentDetails.Where(x => x.PortfolioID == SessionPortFolioID).Count();
                           
                            ViewData["NoOfInstitution"] = db.DocumentDetails.Where(x => x.PortfolioID == SessionPortFolioID).Distinct().GroupBy(o => new
                            {
                                o.Institution
                            }).Count();
                            ViewData["NewAccountAgreement"] = db.DocumentDetails.Where(m => m.Category.Equals("Client Agreements") && m.PortfolioID.Equals(SessionPortFolioID)).Count();
                            ViewData["InvestmentManagementAgreement"] = db.DocumentDetails.Where(m => m.Category.Equals("Investment Agreements") && m.PortfolioID.Equals(SessionPortFolioID)).Count();
                            ViewData["AutomatedFundsTransferAgreement"] = db.DocumentDetails.Where(m => m.Category.Equals("Funds Transfer Agreement") && m.PortfolioID.Equals(SessionPortFolioID)).Count();
                            ViewData["AssetTransferAgreements"] = db.DocumentDetails.Where(m => m.Category.Equals("Asset Transfer Agreements") && m.PortfolioID.Equals(SessionPortFolioID)).Count();
                            ViewData["InsuranceContractAgreements"] = db.DocumentDetails.Where(m => m.Category.Equals("Insurance Agreements") && m.PortfolioID.Equals(SessionPortFolioID)).Count();
                            ViewData["MiscellaneousInvestments"] = db.DocumentDetails.Where(m => m.Category.Equals("Miscellaneous Investments") && m.PortfolioID.Equals(SessionPortFolioID)).Count();
                            ViewData["ClientProfiles"] = db.DocumentDetails.Where(m => m.Category.Equals("Client Profile") && m.PortfolioID.Equals(SessionPortFolioID)).Count();
                            ProfileModel = new ProfileModel
                            {

                                CustomerDetails = db.CustomerDetails.ToList(),
                                PortfolioDetails = db.PortfolioDetails.ToList(),
                                DocumentDetails = db.DocumentDetails.Include("CustomerDetail").Where(m => m.PortfolioID == SessionPortFolioID).ToList(),
                                CategoryDetails = db.CategoryDetails.ToList(),
                                SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),


                            };
                            ViewData["NoofCategory"] = ProfileModel.DocumentDetails.Select(o => o.Category).Distinct().Count();
                        }


                        //Assign the value to ViewBag


                    }
                }

                //   dashboard.PortfolioName = portfolioName;
                return View(ProfileModel);
            }
            else { return RedirectToAction("Login", "Login"); }


        }


        public string LoadPortfolio(int dPortFolioID)
        {
            Session["dPortFolioID"] = dPortFolioID;
            return string.Empty;

        }
        public string LoadCategory(string strCategory)
        {
            Session["strCategory"] = strCategory;
            return string.Empty;

        }
        public string LoadInstitution(string strInstitution)
        {
            Session["strInstitution"] = strInstitution;
            return string.Empty;

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


                // return View(blobs);
                //  docUpload.PortfolioName = portfolioName;
                return View(docUpload);
            }
            else
            { return RedirectToAction("Login", "Login"); }

        }
        [HttpPost]
        public ActionResult DocumentsUpload(HttpPostedFileBase[] files)
        {
            ViewBag.UploadClicked = 1;
            int iPortFolioID = Convert.ToInt32(Session["dPortfolioID"]);
            BlobManager blobManagerObj = new BlobManager("uploadfiles");
            string FileAbsoluteUri;
            string strErrorMessage = string.Empty;
            string strJSONIdentifier = string.Empty;
            DocumentUpload docUpload = new DocumentUpload();
            string strUserEmailID = Session["UserEmailID"].ToString();
            int userId = 0;
            using (docu3cEntities db = new docu3cEntities())
            {
                userId = db.UserDetails.FirstOrDefault(m => m.LoginID.Equals(strUserEmailID)).UserID;

                //Ensure model state is valid  
                if (ModelState.IsValid)
                {   //iterating through multiple file collection  
                    if (iPortFolioID > 0)
                    {
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
                                        // 
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



                                                    CultureInfo invariantCulture = CultureInfo.CreateSpecificCulture("en-US");
                                                    string strReason = string.Empty;
                                                    string exSSN = string.Empty;
                                                    string exAddress = string.Empty;
                                                    string CustomerAddress = string.Empty;
                                                    string CustomerSSN = string.Empty;
                                                    string ExDocumentName = string.Empty;
                                                    // DateTime dtDOB;

                                                    if (!isDocumentAlreadyExists)
                                                    {


                                                        if (docinfo[0].docProps.ContainsKey("cust.addr"))
                                                            CustomerAddress = docinfo[0].docProps["cust.addr"].Value.ToString();
                                                        if (docinfo[0].docProps.ContainsKey("cust.ssn"))
                                                            CustomerSSN = docinfo[0].docProps["cust.ssn"].Value.ToString();

                                                        CustomerDetail nCustomerDetails = new CustomerDetail();

                                                        if (!isDocCustomerAlreadyExists)
                                                        {
                                                            nCustomerDetails.CustomerFirstName = strCustomerName;
                                                            nCustomerDetails.PortfolioID = iPortFolioID;
                                                            nCustomerDetails.AdvisorID = userId;
                                                            nCustomerDetails.IsActive = true;
                                                            nCustomerDetails.CreatedBy = userId.ToString();
                                                            nCustomerDetails.CreatedOn = DateTime.Now;
                                                            db.CustomerDetails.Add(nCustomerDetails);
                                                            db.SaveChanges();


                                                        }
                                                        if (isDocCustomerAlreadyExists)
                                                        {
                                                            List<CustomerDetail> exCustomerDetails = new List<CustomerDetail>();
                                                            exCustomerDetails = db.CustomerDetails.Where(m => m.CustomerFirstName.Equals(strCustomerName)).ToList();
                                                            foreach (var item in exCustomerDetails)
                                                            {
                                                                if (docinfo[0].docProps.ContainsKey("doc.type"))
                                                                    strJSONIdentifier = docinfo[0].docProps["doc.type"].Value.ToString();
                                                                strJSONIdentifier = strJSONIdentifier.Replace("_", " ");

                                                                if (strJSONIdentifier == "Client Relationship Agreement")
                                                                {


                                                                    if (docinfo[0].docProps.ContainsKey("cust.dob"))

                                                                        item.DOB = DateTime.Parse(docinfo[0].docProps["cust.dob"].Value.ToString(), invariantCulture);
                                                                    item.DocCustomerID = CustomerSSN;
                                                                    item.Address = CustomerAddress;
                                                                    item.ModifiedBy = userId.ToString();
                                                                    item.ModifiedOn = DateTime.Now;



                                                                    db.SaveChanges();
                                                                }

                                                            }
                                                        }



                                                        if (docinfo[0].docProps.ContainsKey("cust.ssn"))
                                                            nDocumentDetails.DocCustomerID = docinfo[0].docProps["cust.ssn"].Value.ToString();

                                                        nDocumentDetails.PortfolioID = iPortFolioID;

                                                        nDocumentDetails.UserID = userId;


                                                        nDocumentDetails.CustomerName = strCustomerName;
                                                        nDocumentDetails.Address = CustomerAddress;
                                                        nDocumentDetails.DocumentName = System.IO.Path.GetFileName(docinfo[0].docURL.ToString());
                                                        nDocumentDetails.DocumentURL = docinfo[0].docURL.ToString();

                                                        if (docinfo[0].docProps.ContainsKey("cust.dob"))

                                                            nDocumentDetails.DOB = DateTime.Parse(docinfo[0].docProps["cust.dob"].Value.ToString(), invariantCulture);
                                                        //   nDocumentDetails.Reason = strReason;
                                                        if (docinfo[0].docProps.ContainsKey("doc.type"))
                                                        {
                                                            strJSONIdentifier = docinfo[0].docProps["doc.type"].Value.ToString();
                                                            strJSONIdentifier = strJSONIdentifier.Replace("_", " ");
                                                            strJSONIdentifier = strJSONIdentifier.Replace("®", " ");
                                                            strJSONIdentifier = strJSONIdentifier.Replace("*", "");
                                                            // strJSONIdentifier = string()
                                                            nDocumentDetails.JSONFileIdentifier = strJSONIdentifier;
                                                            strJSONIdentifier = strJSONIdentifier.ToLowerInvariant();

                                                            if (db.CategoryDetails.Any(x => x.JSONIdentifier.Contains(strJSONIdentifier)))
                                                            {
                                                                string strCategoryName = string.Empty;
                                                                List<CategoryDetail> strJSON = new List<CategoryDetail>();
                                                                strJSON = db.CategoryDetails.Where(m => m.JSONIdentifier.Contains(strJSONIdentifier)).ToList();
                                                                foreach (var item in strJSON)

                                                                {
                                                                    Dictionary<string, List<string>> dstrJson = new Dictionary<string, List<string>>();
                                                                    dstrJson.Add(item.CategoryName, item.JSONIdentifier.Split(',').ToList());
                                                                    if (dstrJson.FirstOrDefault().Value.Any(m => m.Equals(strJSONIdentifier, StringComparison.OrdinalIgnoreCase)))
                                                                    {
                                                                        strCategoryName = dstrJson.FirstOrDefault().Key;
                                                                        break;
                                                                    }


                                                                }

                                                                nDocumentDetails.Category = strCategoryName;
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
                                                        //if (string.IsNullOrEmpty(strReason))
                                                        //{
                                                        nDocumentDetails.FileStatus = "Green";
                                                        //}
                                                        //else { nDocumentDetails.FileStatus = "Red"; }
                                                        nDocumentDetails.IsActive = true;
                                                        nDocumentDetails.CreatedBy = userId.ToString();
                                                        nDocumentDetails.CreatedOn = DateTime.Now;
                                                        iCustomerID = db.CustomerDetails.FirstOrDefault(m => m.CustomerFirstName.Equals(strCustomerName)).CustomerID;
                                                        nDocumentDetails.CustomerID = iCustomerID;
                                                        db.DocumentDetails.Add(nDocumentDetails);

                                                        db.SaveChanges();
                                                    }
                                                    else
                                                    {
                                                        strErrorMessage += string.Format("This {0} Document is already exists", System.IO.Path.GetFileName(FileAbsoluteUri));
                                                        // ViewData["ErrorMessage"] =            
                                                        //  return View();

                                                    }
                                                }
                                                else
                                                {
                                                    strErrorMessage += string.Format("Document not recognized: {0}", System.IO.Path.GetFileName(FileAbsoluteUri));
                                                    //ViewData["ErrorMessage"] =
                                                    //return View();


                                                }
                                            }

                                        }

                                    }
                                    else { strErrorMessage += "Document not recognized"; }


                                }



                            }
                            else
                            {
                                strErrorMessage += "Please upload the document again";
                                //  ViewData["ErrorMessage"] = "isDocumentAlreadyExists Uploaded documents are not able to extract the values ";
                                //  return View();
                            }
                        }
                        // return RedirectToAction("DocumentDetails");
                        if (!string.IsNullOrEmpty(strErrorMessage))
                        {
                            ViewData["ErrorMessage"] = strErrorMessage;
                        }
                        else { ViewBag.UploadStatus = files.Count().ToString() + " files uploaded successfully."; return RedirectToAction("DocumentDetails"); }
                        //  ViewBag.UploadStatus = files.Count().ToString() + " files uploaded successfully.";
                    }
                    else { ViewData["ErrorMessage"] = "Please select the Portfolio before uploading the documents"; }

                }
            }
            return View(docUpload);
        }

        public string CheckCompliance()
        {
            if (Session["UserName"] != null && Session["Role"] != null && Session["UserEmailID"] != null)
            {
                int SessionPortFolioID;
                if (Session["dPortFolioID"] != null)
                {
                    SessionPortFolioID = Convert.ToInt32(Session["dPortFolioID"].ToString());
                }
                else { SessionPortFolioID = 0; }
                if (SessionPortFolioID > 0)
                {
                    using (docu3cEntities db = new docu3cEntities())
                    {
                        List<CustomerDetail> customerDetails = new List<CustomerDetail>();
                        List<DocumentDetail> documentDetails = new List<DocumentDetail>();
                        string CRASSN = string.Empty;
                        string CRACustomerName = string.Empty;
                        string CRAAddress = string.Empty;
                        DateTime? CRADOB = null;
                        int CRACustomerID = 0;
                       
                        string strStatus = string.Empty;
                        customerDetails = db.CustomerDetails.Where(m => m.PortfolioID.Equals(SessionPortFolioID)).ToList();
                        foreach (var item in customerDetails)
                        {
                            // if(item.CustomerFirstName!=null)
                            CRACustomerName = item.CustomerFirstName;
                           // if (item.Address != null)
                                CRAAddress = item.Address;
                           // if (item.DocCustomerID != null)
                                CRASSN = item.DocCustomerID;
                          //  if (item.DOB != null)
                                CRADOB = Convert.ToDateTime(item.DOB);
                            CRACustomerID = item.CustomerID;
                            string strReason = string.Empty;
                            documentDetails = db.DocumentDetails.Where(m => m.CustomerID.Equals(CRACustomerID)).ToList();
                            foreach (var docitem in documentDetails)
                            {
                                string strComplianceData = string.Empty;
                                if (string.IsNullOrEmpty(docitem.Reason) && docitem.JSONFileIdentifier != "Client Relationship Agreement")
                                {
                                    string docSSN = docitem.DocCustomerID;
                                    string docCustomerName = docitem.CustomerName;
                                    string docAddress = docitem.Address;

                                    DateTime? docDOB = null;
                                    docDOB = Convert.ToDateTime(docitem.DOB);

                                    if (CRACustomerName != docCustomerName && !string.IsNullOrEmpty(docCustomerName))
                                    { strComplianceData += "Customer Name is mismatch;"; }

                                    if (!string.IsNullOrEmpty(CRASSN))
                                    {
                                        if (!string.IsNullOrEmpty(docSSN))
                                        {
                                            if (CRASSN != docSSN)
                                            { strComplianceData += string.Format("SSN is mismatch;{0}", Environment.NewLine); }
                                        }
                                        else { strComplianceData += string.Format("SSN is missing;{0}", Environment.NewLine); }
                                    }
                                    else { strComplianceData += string.Format("CRA SSN is missing;{0}", Environment.NewLine); }

                                    if (CRADOB.HasValue)
                                    {
                                        if (docDOB.HasValue)
                                        {
                                            if (CRADOB != docDOB)
                                            { strComplianceData += string.Format("DOB is mismatch;{0}", Environment.NewLine); }
                                        }
                                        else { strComplianceData += string.Format("DOB is missing; {0}", Environment.NewLine); }
                                    }
                                    else { strComplianceData += string.Format("CRA DOB is missing; {0}", Environment.NewLine); }
                                    if (!string.IsNullOrEmpty(strComplianceData))
                                    {
                                        if (!string.IsNullOrEmpty(CRAAddress))
                                        {
                                            if (!string.IsNullOrEmpty(docAddress))
                                            {
                                                if (CRAAddress != docAddress)
                                                { strComplianceData += string.Format("Address is mismatch; {0}", Environment.NewLine); docitem.FileStatus = "Red"; }
                                            }
                                            else { strComplianceData += string.Format("Address is missing;{0}", Environment.NewLine); docitem.FileStatus = "Red"; }
                                        }
                                        else { strComplianceData += string.Format("CRA Address is missing;{0}", Environment.NewLine); docitem.FileStatus = "Red"; }
                                       

                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(CRAAddress))
                                        {
                                            if (!string.IsNullOrEmpty(docAddress))
                                            {
                                                if (CRAAddress != docAddress)
                                                { strComplianceData += string.Format("Address is mismatch; {0}", Environment.NewLine); docitem.FileStatus = "Yellow"; }
                                            }
                                            else { strComplianceData += string.Format("Address is missing;{0}", Environment.NewLine); docitem.FileStatus = "Yellow"; }
                                        }
                                        else { strComplianceData += string.Format("CRA Address is missing;{0}", Environment.NewLine); docitem.FileStatus = "Yellow"; }
                                        
                                    }
                                    
                                    docitem.Reason = strComplianceData;

                                    docitem.ModifiedOn = DateTime.Now;
                                    docitem.ModifiedBy = docitem.UserID.ToString();
                                    //db.DocumentDetails.Append(docitem.Reason);

                                    db.SaveChanges();
                                }

                            }

                        }
                        // return RedirectToAction("DocumentDetails", "Home");


                    }


                }
                else { ViewData["ErrorMessage"] = "Please Select the PortFolio to check for Compliance"; }
            }
            return string.Empty;
        }




        public ActionResult About()
        {

            return View();

        }
        public string CategoryQuery(int? queryid)
        {
            string strCategoryName = string.Empty;
            if (queryid == 1)
            {
                strCategoryName = "Client Agreements";
            }
            if (queryid == 2)
            {
                strCategoryName = "Investment Agreements";
            }
            if (queryid == 3)
            {
                strCategoryName = "Funds Transfer Agreement";
            }
            if (queryid == 4)
            {
                strCategoryName = "Asset Transfer Agreements";
            }
            if (queryid == 5)
            {
                strCategoryName = "Insurance Agreements";
            }
            if (queryid == 6)
            {
                strCategoryName = "Miscellaneous Investments";
            }
            if (queryid == 7)
            {
                strCategoryName = "Client Profile";
            }
            return strCategoryName;
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult DocumentDetails(int? queryid)
        {
            ProfileModel ProfileModel = new ProfileModel();
            if (Session["UserName"] != null && Session["Role"] != null && Session["UserEmailID"] != null)
            {
                string strUserEmailID = Session["UserEmailID"].ToString();
                //   int userId = 0;
                int SessionPortFolioID;
                if (Session["dPortFolioID"] != null)
                {
                    SessionPortFolioID = Convert.ToInt32(Session["dPortFolioID"].ToString());
                }
                else { SessionPortFolioID = 0; }
                using (docu3cEntities db = new docu3cEntities())
                {
                    // userId = db.UserDetails.FirstOrDefault(m => m.LoginID.Equals(strUserEmailID)).UserID;
                    //  CDocumentList = db.DocumentDetails.ToList();
                    //portfolioName = db.UserDetails.FirstOrDefault((m => m.LoginID.Equals(Session["UserEmailID"]))).PortfolioDetails.FirstOrDefault().PortfolioName;
                    if (User != null)
                    {
                        string strCategoryName = CategoryQuery(queryid);


                        if (SessionPortFolioID == 0)
                        {
                            // ViewData["PortfolioName"] = db.PortfolioDetails.FirstOrDefault(m => m.UserID.Equals(userId)).PortfolioName;

                            if (!string.IsNullOrEmpty(strCategoryName))
                            {
                                ProfileModel = new ProfileModel
                                {

                                    CustomerDetails = db.CustomerDetails.ToList(),
                                    PortfolioDetails = db.PortfolioDetails.ToList(),
                                    DocumentDetails = db.DocumentDetails.Include("CustomerDetail").Where(m=>m.Category == strCategoryName ).ToList(),

                                    CategoryDetails = db.CategoryDetails.ToList(),
                                    SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                                };

                            }
                            else
                            {

                                ProfileModel = new ProfileModel
                                {

                                    CustomerDetails = db.CustomerDetails.ToList(),
                                    PortfolioDetails = db.PortfolioDetails.ToList(),
                                    DocumentDetails = db.DocumentDetails.Include("CustomerDetail").ToList(),

                                    CategoryDetails = db.CategoryDetails.ToList(),
                                    SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                                };
                            }
                            ViewData["NoofCategory"] = ProfileModel.DocumentDetails.Select(o => o.Category).Distinct().Count();
                            ViewData["NoofCustomers"] = ProfileModel.CustomerDetails.Count();
                            ViewData["NoofDocumets"] = ProfileModel.DocumentDetails.Count();

                            ViewData["NoOfInstitution"] = ProfileModel.DocumentDetails.Distinct().GroupBy(o => new
                            {
                                o.Institution
                            }).Count();
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(strCategoryName))
                            {

                                ProfileModel = new ProfileModel
                                {

                                    CustomerDetails = db.CustomerDetails.Where(x => x.PortfolioID == SessionPortFolioID).ToList(),
                                    PortfolioDetails = db.PortfolioDetails.ToList(),
                                    DocumentDetails = db.DocumentDetails.Include("CustomerDetail").Where(x => x.PortfolioID == SessionPortFolioID && x.Category ==strCategoryName).ToList(),
                                    CategoryDetails = db.CategoryDetails.ToList(),
                                    SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                                };
                            }
                            else
                            {

                                ProfileModel = new ProfileModel
                                {

                                    CustomerDetails = db.CustomerDetails.Where(x => x.PortfolioID == SessionPortFolioID).ToList(),
                                    PortfolioDetails = db.PortfolioDetails.ToList(),
                                    DocumentDetails = db.DocumentDetails.Include("CustomerDetail").Where(x => x.PortfolioID == SessionPortFolioID).ToList(),
                                    CategoryDetails = db.CategoryDetails.ToList(),
                                    SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                                };
                            }
                            ViewData["NoofCategory"] = ProfileModel.DocumentDetails.Select(o => o.Category).Distinct().Count();
                            ViewData["NoofCustomers"] = ProfileModel.CustomerDetails.Where(x => x.PortfolioID == SessionPortFolioID).Count();
                            ViewData["NoofDocumets"] = ProfileModel.DocumentDetails.Where(x => x.PortfolioID == SessionPortFolioID).Count();

                            ViewData["NoOfInstitution"] = ProfileModel.DocumentDetails.Where(x => x.PortfolioID == SessionPortFolioID).Distinct().GroupBy(o => new
                            {
                                o.Institution
                            }).Count();
                        }
                    }
                }
                return View(ProfileModel);
            }
            else { return RedirectToAction("Login", "Login"); }

        }
        public ActionResult CustomerDetails(int? queryid)
        {
            ProfileModel ProfileModel = new ProfileModel();
            if (Session["UserName"] != null && Session["Role"] != null && Session["UserEmailID"] != null)
            {
                string strUserEmailID = Session["UserEmailID"].ToString();
                // int userId = 0;
                int SessionPortFolioID;
                if (Session["dPortFolioID"] != null)
                {
                    SessionPortFolioID = Convert.ToInt32(Session["dPortFolioID"].ToString());
                }
                else { SessionPortFolioID = 0; }

                using (docu3cEntities db = new docu3cEntities())
                {

                    if (User != null)
                    {
                        string strCategoryName = CategoryQuery(queryid);
                        if (SessionPortFolioID == 0)
                        {

                            if (!string.IsNullOrEmpty(strCategoryName))
                            {
                                ProfileModel = new ProfileModel
                                {

                                    CustomerDetails = db.CustomerDetails.Where(m => m.DocumentDetails.FirstOrDefault().Category == strCategoryName).ToList(),
                                    PortfolioDetails = db.PortfolioDetails.ToList(),
                                    DocumentDetails = db.DocumentDetails.Include("CustomerDetail").ToList(),
                                    CategoryDetails = db.CategoryDetails.ToList(),
                                    SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                                };
                            }else
                            {
                                ProfileModel = new ProfileModel
                                {

                                    CustomerDetails = db.CustomerDetails.ToList(),
                                    PortfolioDetails = db.PortfolioDetails.ToList(),
                                    DocumentDetails = db.DocumentDetails.Include("CustomerDetail").ToList(),
                                    CategoryDetails = db.CategoryDetails.ToList(),
                                    SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                                };
                            }
                            ViewData["NoofCategory"] = ProfileModel.DocumentDetails.Select(o => o.Category).Distinct().Count();
                            ViewData["NoofCustomers"] = ProfileModel.CustomerDetails.Count();
                            ViewData["NoofDocumets"] = ProfileModel.DocumentDetails.Count();

                            ViewData["NoOfInstitution"] = ProfileModel.DocumentDetails.Distinct().GroupBy(o => new
                            {
                                o.Institution
                            }).Count();
                        }
                        else
                        {

                            if (!string.IsNullOrEmpty(strCategoryName))
                            {
                                ProfileModel = new ProfileModel
                                {

                                    CustomerDetails = db.CustomerDetails.Where(x => x.PortfolioID == SessionPortFolioID).ToList(),
                                    PortfolioDetails = db.PortfolioDetails.ToList(),
                                    DocumentDetails = db.DocumentDetails.Include("CustomerDetail").Where(x => x.PortfolioID == SessionPortFolioID && x.Category == strCategoryName).ToList(),
                                    CategoryDetails = db.CategoryDetails.ToList(),
                                    SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                                };
                            }
                            else {
                                ProfileModel = new ProfileModel
                                {

                                    CustomerDetails = db.CustomerDetails.Where(x => x.PortfolioID == SessionPortFolioID).ToList(),
                                    PortfolioDetails = db.PortfolioDetails.ToList(),
                                    DocumentDetails = db.DocumentDetails.Include("CustomerDetail").Where(x => x.PortfolioID == SessionPortFolioID).ToList(),
                                    CategoryDetails = db.CategoryDetails.ToList(),
                                    SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                                };
                            }
                            ViewData["NoofCategory"] = ProfileModel.DocumentDetails.Select(o => o.Category).Distinct().Count();
                            ViewData["NoofCustomers"] = ProfileModel.CustomerDetails.Where(x => x.PortfolioID == SessionPortFolioID).Count();
                            ViewData["NoofDocumets"] = ProfileModel.DocumentDetails.Where(x => x.PortfolioID == SessionPortFolioID).Count();

                            ViewData["NoOfInstitution"] = ProfileModel.DocumentDetails.Where(x => x.PortfolioID == SessionPortFolioID).Distinct().GroupBy(o => new
                            {
                                o.Institution
                            }).Count();

                        }
                    }
                }
                return View(ProfileModel);
            }
            else { return RedirectToAction("Login", "Login"); }

        }

        public ActionResult InstitutionDetails(int? queryid)
        {
            ProfileModel ProfileModel = new ProfileModel();
            if (Session["UserName"] != null && Session["Role"] != null && Session["UserEmailID"] != null)
            {
                string strUserEmailID = Session["UserEmailID"].ToString();
                //  int userId = 0;
                using (docu3cEntities db = new docu3cEntities())
                {
                    //  userId = db.UserDetails.FirstOrDefault(m => m.LoginID.Equals(strUserEmailID)).UserID;
                    //  CDocumentList = db.DocumentDetails.ToList();
                    //portfolioName = db.UserDetails.FirstOrDefault((m => m.LoginID.Equals(Session["UserEmailID"]))).PortfolioDetails.FirstOrDefault().PortfolioName;
                    if (User != null)
                    {
                        string strCategoryName = CategoryQuery(queryid);
                        int SessionPortFolioID;
                        if (Session["dPortFolioID"] != null)
                        {
                            SessionPortFolioID = Convert.ToInt32(Session["dPortFolioID"].ToString());
                        }
                        else { SessionPortFolioID = 0; }
                        if (SessionPortFolioID == 0)
                        {
                            //ViewData["PortfolioName"] = db.PortfolioDetails.FirstOrDefault(m => m.UserID.Equals(userId)).PortfolioName;

                            if (!string.IsNullOrEmpty(strCategoryName))
                            {
                                ProfileModel = new ProfileModel
                                {

                                    CustomerDetails = db.CustomerDetails.ToList(),
                                    PortfolioDetails = db.PortfolioDetails.ToList(),
                                    DocumentDetails = db.DocumentDetails.Where(m => m.Category == strCategoryName).ToList(),

                                    CategoryDetails = db.CategoryDetails.ToList(),
                                    SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                                };
                            }
                            else
                            {
                                ProfileModel = new ProfileModel
                                {

                                    CustomerDetails = db.CustomerDetails.ToList(),
                                    PortfolioDetails = db.PortfolioDetails.ToList(),
                                    DocumentDetails = db.DocumentDetails.ToList(),

                                    CategoryDetails = db.CategoryDetails.ToList(),
                                    SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                                };
                            }
                            ViewData["NoofCategory"] = ProfileModel.DocumentDetails.Select(o => o.Category).Distinct().Count();
                            ViewData["NoofCustomers"] = ProfileModel.CustomerDetails.Count();
                            ViewData["NoofDocumets"] = ProfileModel.DocumentDetails.Count();

                            ViewData["NoOfInstitution"] = ProfileModel.DocumentDetails.Distinct().GroupBy(o => new
                            {
                                o.Institution
                            }).Count();
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(strCategoryName))
                            {

                                ProfileModel = new ProfileModel
                                {

                                    CustomerDetails = db.CustomerDetails.Where(x => x.PortfolioID == SessionPortFolioID).ToList(),
                                    PortfolioDetails = db.PortfolioDetails.ToList(),
                                    DocumentDetails = db.DocumentDetails.Where(x => x.PortfolioID == SessionPortFolioID && x.Category==strCategoryName).ToList(),

                                    CategoryDetails = db.CategoryDetails.ToList(),
                                    SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                                };
                            }
                            else
                            {
                                ProfileModel = new ProfileModel
                                {

                                    CustomerDetails = db.CustomerDetails.Where(x => x.PortfolioID == SessionPortFolioID).ToList(),
                                    PortfolioDetails = db.PortfolioDetails.ToList(),
                                    DocumentDetails = db.DocumentDetails.Where(x => x.PortfolioID == SessionPortFolioID).ToList(),

                                    CategoryDetails = db.CategoryDetails.ToList(),
                                    SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                                };
                            }
                            ViewData["NoofCategory"] = ProfileModel.DocumentDetails.Select(o => o.Category).Distinct().Count();
                            ViewData["NoofCustomers"] = ProfileModel.CustomerDetails.Where(x => x.PortfolioID == SessionPortFolioID).Count();
                            ViewData["NoofDocumets"] = ProfileModel.DocumentDetails.Where(x => x.PortfolioID == SessionPortFolioID).Count();

                            ViewData["NoOfInstitution"] = ProfileModel.DocumentDetails.Where(x => x.PortfolioID == SessionPortFolioID).Distinct().GroupBy(o => new
                            {
                                o.Institution
                            }).Count();
                        }
                    }
                }
                return View(ProfileModel);
            }
            else { return RedirectToAction("Login", "Login"); }

        }

        public ActionResult AddPortFolio()
        { return View(); }
        [HttpPost]
        public ActionResult AddPortFolio(string portfolioname)
        {

            if (Session["UserName"] != null && Session["Role"] != null && Session["UserEmailID"] != null)
            {
                ProfileModel ProfileModel = new ProfileModel();
                if (!string.IsNullOrEmpty(portfolioname))
                {
                    string strUserEmailID = Session["UserEmailID"].ToString();
                    int userId = 0;
                    using (docu3cEntities db = new docu3cEntities())
                    {
                        userId = db.UserDetails.FirstOrDefault(m => m.LoginID.Equals(strUserEmailID)).UserID;


                        PortfolioDetail portfolioDetails = new PortfolioDetail();
                        portfolioDetails.PortfolioName = portfolioname;
                        portfolioDetails.IsActive = true;
                        portfolioDetails.CreatedOn = DateTime.Now;
                        portfolioDetails.CreatedBy = userId.ToString();
                        portfolioDetails.UserID = Convert.ToInt32(userId.ToString());
                        db.PortfolioDetails.Add(portfolioDetails);
                        db.SaveChanges();
                        RedirectToAction("PortFolioDetails", "Home");
                    }
                }
                return View(ProfileModel);
            }

            else { return RedirectToAction("Login", "Login"); }
        }

        public ActionResult PortFolioDetails()
        {
            ProfileModel ProfileModel = new ProfileModel();
            if (Session["UserName"] != null && Session["Role"] != null && Session["UserEmailID"] != null)
            {
                using (docu3cEntities db = new docu3cEntities())
                {
                    ProfileModel = new ProfileModel
                    {

                        CustomerDetails = db.CustomerDetails.ToList(),
                        PortfolioDetails = db.PortfolioDetails.ToList(),
                        DocumentDetails = db.DocumentDetails.ToList(),

                        CategoryDetails = db.CategoryDetails.ToList(),
                        SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                    };
                }
                return View(ProfileModel);
            }
            else { return RedirectToAction("Login", "Login"); }
        }

        public ActionResult Delete(string DocumentID)
        {

            using (docu3cEntities db = new docu3cEntities())
            {
                string strFileAbsoluteUri = string.Empty;
                int iDocumentID = Convert.ToInt32(DocumentID);
                strFileAbsoluteUri = db.DocumentDetails.FirstOrDefault(m => m.DocumentID.Equals(iDocumentID)).DocumentURL;
                DocumentDetail documentDetail = db.DocumentDetails.Find(iDocumentID);

                //  db.UserDetails.FirstOrDefault(m => m.LoginID.Equals(strUserEmailID)).UserID

                db.DocumentDetails.Remove(documentDetail);
                db.SaveChanges();

                //   strFileAbsoluteUri = db.DocumentDetails.Where(x => x.DocumentID.Equals("iDocumentID")).FirstOrDefault().DocumentURL;
                docu3c.BlobHandling.BlobManager blobManager = new BlobManager("uploadfiles");
                blobManager.DeleteBlob(strFileAbsoluteUri);
            }

            return RedirectToAction("DocumentDetails");
        }

        public ActionResult Compliance()
        {
            ProfileModel ProfileModel = new ProfileModel();
            if (Session["UserName"] != null && Session["Role"] != null && Session["UserEmailID"] != null)
            {
                string strUserEmailID = Session["UserEmailID"].ToString();

                using (docu3cEntities db = new docu3cEntities())
                {
                    if (User != null)
                    {
                        int SessionPortFolioID;
                        if (Session["dPortFolioID"] != null)
                        {
                            SessionPortFolioID = Convert.ToInt32(Session["dPortFolioID"].ToString());
                        }
                        else { SessionPortFolioID = 0; }
                        if (SessionPortFolioID == 0)
                        {
                            var needsVerification = db.DocumentDetails.Where(x => x.FileStatus.Equals("Yellow")).Count();
                            var meetCompliance = db.DocumentDetails.Count();
                            ViewData["NeedAttention"] = db.DocumentDetails.Where(x => x.FileStatus.Equals("Red")).Count();
                            ViewData["NeedVerification"] = needsVerification;
                            ViewData["MeetCompliance"] = meetCompliance;
                            ViewData["MeetCompliance"] = meetCompliance; if (meetCompliance > 0)
                            {
                                var complianceScore = (Convert.ToDecimal(needsVerification) / Convert.ToDecimal(meetCompliance)) * 100;
                                ViewData["ComplianceScore"] = Convert.ToInt32(complianceScore);
                            }
                            else { ViewData["ComplianceScore"] = 0; }

                            ProfileModel = new ProfileModel
                            {

                                CustomerDetails = db.CustomerDetails.ToList(),
                                PortfolioDetails = db.PortfolioDetails.ToList(),
                                DocumentDetails = db.DocumentDetails.ToList(),

                                CategoryDetails = db.CategoryDetails.ToList(),
                                SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                            };
                        }
                        else
                        {
                            ViewData["NeedAttention"] = db.DocumentDetails.Where(x => x.FileStatus.Equals("Red") && x.PortfolioID.Equals(SessionPortFolioID)).Count();
                            var needsVerification = db.DocumentDetails.Where(a => a.FileStatus.Equals("Yellow") && a.PortfolioID.Equals(SessionPortFolioID)).Count();
                            var meetCompliance = db.DocumentDetails.Where(m => m.PortfolioID.Equals(SessionPortFolioID)).Count();
                            ViewData["NeedVerification"] = needsVerification;
                            ViewData["MeetCompliance"] = meetCompliance;
                            ViewData["MeetCompliance"] = meetCompliance; if (meetCompliance > 0)
                            {
                                var complianceScore = (Convert.ToDecimal(needsVerification) / Convert.ToDecimal(meetCompliance)) * 100;
                                ViewData["ComplianceScore"] = Convert.ToInt32(complianceScore);
                            }
                            else { ViewData["ComplianceScore"] = 0; }

                            ProfileModel = new ProfileModel
                            {

                                CustomerDetails = db.CustomerDetails.Where(x => x.PortfolioID == SessionPortFolioID).ToList(),
                                PortfolioDetails = db.PortfolioDetails.ToList(),
                                DocumentDetails = db.DocumentDetails.Where(x => x.PortfolioID == SessionPortFolioID).ToList(),

                                CategoryDetails = db.CategoryDetails.ToList(),
                                SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                            };
                        }
                    }
                }
                return View(ProfileModel);
            }
            else { return RedirectToAction("Login", "Login"); }

        }

        public ActionResult Attention()
        {
            ProfileModel ProfileModel = new ProfileModel();
            if (Session["UserName"] != null && Session["Role"] != null && Session["UserEmailID"] != null)
            {
                string strUserEmailID = Session["UserEmailID"].ToString();

                using (docu3cEntities db = new docu3cEntities())
                {
                    if (User != null)
                    {
                        int SessionPortFolioID;
                        if (Session["dPortFolioID"] != null)
                        {
                            SessionPortFolioID = Convert.ToInt32(Session["dPortFolioID"].ToString());
                        }
                        else { SessionPortFolioID = 0; }
                        if (SessionPortFolioID == 0)
                        {
                            var needsVerification = db.DocumentDetails.Where(x => x.FileStatus.Equals("Yellow")).Count();
                            var meetCompliance = db.DocumentDetails.Count();
                            ViewData["NeedAttention"] = db.DocumentDetails.Where(x => x.FileStatus.Equals("Red")).Count();
                            ViewData["NeedVerification"] = needsVerification;
                            ViewData["MeetCompliance"] = meetCompliance;
                            ViewData["MeetCompliance"] = meetCompliance; if (meetCompliance > 0)
                            {
                                var complianceScore = (Convert.ToDecimal(needsVerification) / Convert.ToDecimal(meetCompliance)) * 100;
                                ViewData["ComplianceScore"] = Convert.ToInt32(complianceScore);
                            }
                            else { ViewData["ComplianceScore"] = 0; }
                            ProfileModel = new ProfileModel
                            {

                                CustomerDetails = db.CustomerDetails.ToList(),
                                PortfolioDetails = db.PortfolioDetails.ToList(),
                                DocumentDetails = db.DocumentDetails.Where(x => x.FileStatus.Equals("Red")).ToList(),

                                CategoryDetails = db.CategoryDetails.ToList(),
                                SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                            };
                        }
                        else
                        {

                            ViewData["NeedAttention"] = db.DocumentDetails.Where(x => x.FileStatus.Equals("Red") && x.PortfolioID.Equals(SessionPortFolioID)).Count();
                            var needsVerification = db.DocumentDetails.Where(a => a.FileStatus.Equals("Yellow") && a.PortfolioID.Equals(SessionPortFolioID)).Count();
                            var meetCompliance = db.DocumentDetails.Where(m => m.PortfolioID.Equals(SessionPortFolioID)).Count();
                            ViewData["NeedVerification"] = needsVerification;
                            ViewData["MeetCompliance"] = meetCompliance;
                            ViewData["MeetCompliance"] = meetCompliance; if (meetCompliance > 0)
                            {
                                var complianceScore = (Convert.ToDecimal(needsVerification) / Convert.ToDecimal(meetCompliance)) * 100;
                                ViewData["ComplianceScore"] = Convert.ToInt32(complianceScore);
                            }
                            else { ViewData["ComplianceScore"] = 0; }
                            ProfileModel = new ProfileModel
                            {

                                CustomerDetails = db.CustomerDetails.Where(x => x.PortfolioID == SessionPortFolioID).ToList(),
                                PortfolioDetails = db.PortfolioDetails.ToList(),
                                DocumentDetails = db.DocumentDetails.Where(x => x.PortfolioID == SessionPortFolioID && x.FileStatus.Equals("Red")).ToList(),

                                CategoryDetails = db.CategoryDetails.ToList(),
                                SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                            };
                        }
                    }
                }
                return View(ProfileModel);
            }
            else { return RedirectToAction("Login", "Login"); }

        }

        public ActionResult Verification()
        {
            ProfileModel ProfileModel = new ProfileModel();
            if (Session["UserName"] != null && Session["Role"] != null && Session["UserEmailID"] != null)
            {
                string strUserEmailID = Session["UserEmailID"].ToString();

                using (docu3cEntities db = new docu3cEntities())
                {
                    if (User != null)
                    {
                        int SessionPortFolioID;
                        if (Session["dPortFolioID"] != null)
                        {
                            SessionPortFolioID = Convert.ToInt32(Session["dPortFolioID"].ToString());
                        }
                        else { SessionPortFolioID = 0; }
                        if (SessionPortFolioID == 0)
                        {
                            var needsVerification = db.DocumentDetails.Where(x => x.FileStatus.Equals("Yellow")).Count();
                            var meetCompliance = db.DocumentDetails.Count();
                            ViewData["NeedAttention"] = db.DocumentDetails.Where(x => x.FileStatus.Equals("Red")).Count();
                            ViewData["NeedVerification"] = needsVerification;
                            ViewData["MeetCompliance"] = meetCompliance;
                            ViewData["MeetCompliance"] = meetCompliance; if (meetCompliance > 0)
                            {
                                var complianceScore = (Convert.ToDecimal(needsVerification) / Convert.ToDecimal(meetCompliance)) * 100;
                                ViewData["ComplianceScore"] = Convert.ToInt32(complianceScore);
                            }
                            else { ViewData["ComplianceScore"] = 0; }
                            ProfileModel = new ProfileModel
                            {

                                CustomerDetails = db.CustomerDetails.ToList(),
                                PortfolioDetails = db.PortfolioDetails.ToList(),
                                DocumentDetails = db.DocumentDetails.Where(x => x.FileStatus.Equals("Yellow")).ToList(),

                                CategoryDetails = db.CategoryDetails.ToList(),
                                SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                            };
                        }
                        else
                        {
                            ViewData["NeedAttention"] = db.DocumentDetails.Where(x => x.FileStatus.Equals("Red") && x.PortfolioID.Equals(SessionPortFolioID)).Count();
                            var needsVerification = db.DocumentDetails.Where(a => a.FileStatus.Equals("Yellow") && a.PortfolioID.Equals(SessionPortFolioID)).Count();
                            var meetCompliance = db.DocumentDetails.Where(m => m.PortfolioID.Equals(SessionPortFolioID)).Count();
                            ViewData["NeedVerification"] = needsVerification;
                            ViewData["MeetCompliance"] = meetCompliance;
                            ViewData["MeetCompliance"] = meetCompliance; if (meetCompliance > 0)
                            {
                                var complianceScore = (Convert.ToDecimal(needsVerification) / Convert.ToDecimal(meetCompliance)) * 100;
                                ViewData["ComplianceScore"] = Convert.ToInt32(complianceScore);
                            }
                            else { ViewData["ComplianceScore"] = 0; }
                            ProfileModel = new ProfileModel
                            {

                                CustomerDetails = db.CustomerDetails.Where(x => x.PortfolioID == SessionPortFolioID).ToList(),
                                PortfolioDetails = db.PortfolioDetails.ToList(),
                                DocumentDetails = db.DocumentDetails.Where(x => x.PortfolioID == SessionPortFolioID && x.FileStatus.Equals("Yellow")).ToList(),

                                CategoryDetails = db.CategoryDetails.ToList(),
                                SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                            };
                        }
                    }
                }
                return View(ProfileModel);
            }
            else { return RedirectToAction("Login", "Login"); }

        }



        public ActionResult ComplianceScore()
        {
            ProfileModel profileModel = new ProfileModel();
            if (Session["UserName"] != null && Session["Role"] != null && Session["UserEmailID"] != null)
            {
                JsonSerializerSettings _jsonSetting = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };

                using (docu3cEntities db = new docu3cEntities())
                {
                    if (User != null)
                    {
                        int SessionPortFolioID;
                        if (Session["dPortFolioID"] != null)
                        {
                            SessionPortFolioID = Convert.ToInt32(Session["dPortFolioID"].ToString());
                        }
                        else
                        {
                            SessionPortFolioID = 0;
                        }

                        string SessionCategory = string.Empty;
                        string SessionInstitution = string.Empty;

                        if (Session["strCategory"] != null)
                            SessionCategory = Session["strCategory"].ToString();
                        if (Session["strInstitution"] != null)
                            SessionInstitution = Session["strInstitution"].ToString();


                        if (SessionPortFolioID == 0)
                        {
                            var needsVerification = db.DocumentDetails.Where(x => x.FileStatus.Equals("Yellow")).Count();
                            var meetCompliance = db.DocumentDetails.Count();
                            ViewData["NeedAttention"] = db.DocumentDetails.Where(x => x.FileStatus.Equals("Red")).Count();
                            ViewData["NeedVerification"] = needsVerification;
                            ViewData["MeetCompliance"] = meetCompliance;
                            ViewData["MeetCompliance"] = meetCompliance; if (meetCompliance > 0)
                            {
                                var complianceScore = (Convert.ToDecimal(needsVerification) / Convert.ToDecimal(meetCompliance)) * 100;
                                ViewData["ComplianceScore"] = Convert.ToInt32(complianceScore);
                            }
                            else { ViewData["ComplianceScore"] = 0; }

                            profileModel = new ProfileModel
                            {

                                CustomerDetails = db.CustomerDetails.ToList(),
                                PortfolioDetails = db.PortfolioDetails.ToList(),
                                DocumentDetails = db.DocumentDetails.ToList(),
                                ddCategoryCompliance = db.DocumentDetails.ToList(),
                                ddInstitutionCompliance = db.DocumentDetails.ToList(),
                                CategoryDetails = db.CategoryDetails.ToList()
                            };


                            if (!string.IsNullOrEmpty(SessionCategory))
                                profileModel.ddCategoryCompliance = db.DocumentDetails.Where(x => x.Category.Equals(SessionCategory)).ToList();

                            if (!string.IsNullOrEmpty(SessionInstitution))
                                profileModel.ddInstitutionCompliance = db.DocumentDetails.Where(x => x.Institution.Equals(SessionInstitution)).ToList();

                        }
                        else
                        {
                            ViewData["NeedAttention"] = db.DocumentDetails.Where(x => x.FileStatus.Equals("Red") && x.PortfolioID.Equals(SessionPortFolioID)).Count();
                            var needsVerification = db.DocumentDetails.Where(a => a.FileStatus.Equals("Yellow") && a.PortfolioID.Equals(SessionPortFolioID)).Count();
                            var meetCompliance = db.DocumentDetails.Where(m => m.PortfolioID.Equals(SessionPortFolioID)).Count();
                            ViewData["NeedVerification"] = needsVerification;
                            ViewData["MeetCompliance"] = meetCompliance; if (meetCompliance > 0)
                            { 
                            var complianceScore = (Convert.ToDecimal(needsVerification) / Convert.ToDecimal(meetCompliance)) * 100;
                            ViewData["ComplianceScore"] = Convert.ToInt32(complianceScore);
                        }
                            else { ViewData["ComplianceScore"] = 0; }

                            profileModel = new ProfileModel
                            {

                                CustomerDetails = db.CustomerDetails.Where(x => x.PortfolioID == SessionPortFolioID).ToList(),
                                PortfolioDetails = db.PortfolioDetails.ToList(),
                                ddInstitutionCompliance = db.DocumentDetails.Where(x => x.PortfolioID == SessionPortFolioID).ToList(),
                                ddCategoryCompliance = db.DocumentDetails.Where(x => x.PortfolioID == SessionPortFolioID).ToList(),
                                CategoryDetails = db.CategoryDetails.ToList(),
                                SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                                DocumentDetails = db.DocumentDetails.Where(x => x.PortfolioID == SessionPortFolioID).ToList(),
                            };

                            if (!string.IsNullOrEmpty(SessionCategory))
                                profileModel.ddCategoryCompliance = db.DocumentDetails.Where(x => x.PortfolioID == SessionPortFolioID && x.Category.Equals(SessionCategory)).ToList();

                            if (!string.IsNullOrEmpty(SessionInstitution))
                                profileModel.ddInstitutionCompliance = db.DocumentDetails.Where(x => x.PortfolioID == SessionPortFolioID && x.Institution.Equals(SessionInstitution)).ToList();

                        }
                    }
                }
                return View(profileModel);
            }
            else { return RedirectToAction("Login", "Login"); }

        }
    }

}
