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
           
           
            DocumentUpload docUpload = new DocumentUpload();
            string strUserEmailID = Session["UserEmailID"].ToString();
            int userId = 0;
            using (docu3cEntities db = new docu3cEntities())
            {
                userId = db.UserDetails.FirstOrDefault(m => m.LoginID.Equals(strUserEmailID)).UserID;

                //Ensure model state is valid  
                if (ModelState.IsValid)
                {   //iterating through multiple file collection  
                    string strErrorMessage = string.Empty;
                   // string strJSONIdentifier = string.Empty;
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

                                string FileAbsoluteUri = string.Empty;
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
                                                string SERVICE_URL = "https://docu3c-modelservice.azurewebsites.net/?url=";
                                                //  docu3cAPIClient d3 = new docu3cAPIClient();
                                                string fileurl = "https://docu3capp.blob.core.windows.net/uploadfiles/";
                                                HttpClient client = new HttpClient();
                                                client.BaseAddress = new Uri(SERVICE_URL + FileAbsoluteUri);
                                                var request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                                                
                                                var response = client.SendAsync(request).Result;
                                                var jsonString = response.Content.ReadAsStringAsync().Result;
                                              //  jsonString = Regex.Replace(jsonString, @"(""[^""\\]*(?:\\.[^""\\]*)*"")|\s+", "$1");
                                              var  docinfo = JsonConvert.DeserializeObject<docu3cAPI>(jsonString);
                                                //if (docinfo[0].docParseErrorMsg != null)
                                                //{
                                                //    ViewBag.ErrorMsg = "The Uploaded document is not a valid form";
                                                //}

                                                if (docinfo != null)
                                                {
                                                    string strCustomerFirstName = string.Empty;
                                                    string strCustomerLastName = string.Empty;
                                                    string strCustomerMiddleName = string.Empty;
                                                    string strCustomerName = string.Empty;
                                                    string docURL = string.Empty;
                                                    string strReason = string.Empty;
                                                    int iCustomerID = 0;


                                                   // if (!string.IsNullOrEmpty(docinfo[0].docURL.ToString()))
                                                        docURL = FileAbsoluteUri;
                                                    if (string.IsNullOrEmpty(docinfo.details.FirstName.ToString() ))
                                                     strCustomerFirstName = docinfo.details.FirstName.ToString(); 
                                                    
                                                    if (string.IsNullOrEmpty(docinfo.details.MiddleName.ToString()))
                                                        strCustomerMiddleName = docinfo.details.MiddleName.ToString();
                                                    if (string.IsNullOrEmpty(docinfo.details.LastName.ToString()))
                                                        strCustomerLastName = docinfo.details.LastName.ToString();
                                                    if (string.IsNullOrEmpty(strCustomerFirstName) && string.IsNullOrEmpty(strCustomerLastName))
                                                    {
                                                        strCustomerName = string.Format("{0} {1} {2}", strCustomerFirstName, strCustomerMiddleName, strCustomerLastName);
                                                    }
                                                    else { strReason = "Name is missing"; }
                                                    var isDocCustomerAlreadyExists = db.DocumentDetails.Any(x => x.CustomerName == strCustomerName);
                                                   
                                                    var isDocumentAlreadyExists = db.DocumentDetails.Any(x => x.DocumentURL == docURL);
                                                    DocumentDetail nDocumentDetails = new DocumentDetail();



                                                    CultureInfo invariantCulture = CultureInfo.CreateSpecificCulture("en-US");
                                                   
                                                    string exSSN = string.Empty;
                                                    string exAddress = string.Empty;
                                                    string docAddress = string.Empty;
                                                    string docSSN = string.Empty;
                                                    string docState = string.Empty;
                                                    string docCity = string.Empty;
                                                    string docPostalCode = string.Empty;
                                                    string docJSONIdentifier = string.Empty;
                                                    string docSubcategory = string.Empty;
                                                    string docOrganisation = string.Empty;
                                                    string ExDocumentName = string.Empty;
                                                    string docAccountNo = string.Empty;
                                                    string[] formats = { "dd.MM.yyyy", "dd-MM-yyyy", "dd/MM/yyyy" };
                                                    DateTime dtDOB;
                                                    nDocumentDetails.ClassifyJSON =jsonString;
                                                    if (string.IsNullOrEmpty(docinfo.details.Street))
                                                    
                                                        docAddress = docinfo.details.Street.ToString();
                                                   
                                                    
                                                    if (string.IsNullOrEmpty(docinfo.details.City))
                                                        docCity = docinfo.details.City.ToString();
                                                    if (string.IsNullOrEmpty(docinfo.details.State))
                                                        docState = docinfo.details.State.ToString();
                                                    if (string.IsNullOrEmpty(docinfo.details.Zipcode))
                                                        docPostalCode = docinfo.details.Zipcode.ToString();
                                                    if(string.IsNullOrEmpty(docAddress) || string.IsNullOrEmpty(docCity) || string.IsNullOrEmpty(docState) || string.IsNullOrEmpty(docPostalCode))
                                                    { strReason += string.Format("{0}Address is missing", Environment.NewLine); }

                                                    if (string.IsNullOrEmpty(docinfo.details.Organisation ))
                                                    {
                                                        docOrganisation = docinfo.details.Organisation.ToString(); }
                                                    else {  strReason += string.Format("{0}Institution is missing", Environment.NewLine);  }
                                                    if (string.IsNullOrEmpty(docinfo.details.SSN))
                                                    {
                                                        docSSN = docinfo.details.SSN.ToString();
                                                    }
                                                    else {  strReason += string.Format("{0}SSN is missing", Environment.NewLine);  }
                                                    if (string.IsNullOrEmpty(docinfo.details.Category))
                                                        docJSONIdentifier = docinfo.details.Category.ToString();
                                                    if (string.IsNullOrEmpty(docinfo.SubCategory))
                                                    {
                                                        docSubcategory = docinfo.SubCategory.ToString();
                                                    }
                                                    else { strReason += string.Format("{0}Asset is missing", Environment.NewLine); }
                                                    if (string.IsNullOrEmpty(docinfo.details.AccountNo))
                                                    {
                                                        docAccountNo = docinfo.details.AccountNo.ToString();
                                                    }
                                                    else { strReason += string.Format("{0}Account No. is missing", Environment.NewLine); }


                                                    if (!isDocumentAlreadyExists)
                                                {


                                                    

                                                    CustomerDetail nCustomerDetails = new CustomerDetail();

                                                    if (!isDocCustomerAlreadyExists)
                                                    {
                                                        nCustomerDetails.CustomerFirstName = strCustomerFirstName;
                                                        nCustomerDetails.CustomerMiddleName = strCustomerMiddleName;
                                                        nCustomerDetails.CustomerLastName = strCustomerLastName;
                                                      //  nCustomerDetails.AccountNo = docAccountNo;
                                                        nCustomerDetails.PortfolioID = iPortFolioID;
                                                        nCustomerDetails.AdvisorID = userId;
                                                        nCustomerDetails.IsActive = true;
                                                        nCustomerDetails.CreatedBy = userId.ToString();
                                                        nCustomerDetails.CreatedOn = DateTime.Now;
                                                            if (docJSONIdentifier == "Client Relationship Agreement")
                                                            {

                                                                if (string.IsNullOrEmpty(docinfo.details.DOB.ToString()))
                                                                {

                                                                    bool isValidDOB = DateTime.TryParseExact(docinfo.details.DOB.ToString(), formats, invariantCulture, DateTimeStyles.None, out dtDOB);
                                                                    if (isValidDOB)
                                                                    { nCustomerDetails.DOB = DateTime.Parse(docinfo.details.DOB.ToString(), invariantCulture); }


                                                                }
                                                                nCustomerDetails.DocCustomerID = docSSN;
                                                                nCustomerDetails.Address = docAddress;
                                                                nCustomerDetails.State = docState;
                                                                nCustomerDetails.City = docCity;
                                                                nCustomerDetails.PostalCode = docPostalCode;
                                                                nCustomerDetails.AccountNo = docAccountNo;


                                                            }


                                                                db.CustomerDetails.Add(nCustomerDetails);
                                                        db.SaveChanges();


                                                    }
                                                    if (isDocCustomerAlreadyExists)
                                                    {
                                                        List<CustomerDetail> exCustomerDetails = new List<CustomerDetail>();
                                                        exCustomerDetails = db.CustomerDetails.Where(m => m.CustomerFirstName.Equals(strCustomerFirstName) && m.CustomerMiddleName.Equals(strCustomerMiddleName) &&m.CustomerLastName .Equals(strCustomerLastName)).ToList();
                                                        foreach (var item in exCustomerDetails)
                                                        {
                                                           
                                                          //  strJSONIdentifier = strJSONIdentifier.Replace("_", " ");

                                                            if (docJSONIdentifier == "Client Relationship Agreement")
                                                            {

                                                                if (string.IsNullOrEmpty(docinfo.details.DOB.ToString()))
                                                                {

                                                                        item.DOB = DateTime.Parse(docinfo.details.DOB.ToString(), invariantCulture);


                                                                    }
                                                                item.DocCustomerID = docSSN;
                                                                item.Address = docAddress;
                                                                    item.State = docState;
                                                                    item.City = docCity;
                                                                    
                                                                    item.PostalCode = docPostalCode;
                                                                    item.AccountNo = docAccountNo;
                                                                item.ModifiedBy = userId.ToString();
                                                                item.ModifiedOn = DateTime.Now;



                                                                db.SaveChanges();
                                                            }

                                                        }
                                                    }



                                                    if (string.IsNullOrEmpty(docSSN))
                                                    nDocumentDetails.DocCustomerID = docSSN;
                                                    nDocumentDetails.PortfolioID = iPortFolioID;
                                                    nDocumentDetails.UserID = userId;
                                                    if(string.IsNullOrEmpty(strCustomerName))
                                                    nDocumentDetails.CustomerName = strCustomerName;
                                                       
                                                            if (string.IsNullOrEmpty(docAddress))
                                                    nDocumentDetails.Address = docAddress;
                                                        if (string.IsNullOrEmpty(docCity))
                                                            nDocumentDetails.City  = docCity;
                                                        if (string.IsNullOrEmpty(docState ))
                                                            nDocumentDetails.State = docState;
                                                        if (string.IsNullOrEmpty(docPostalCode ))
                                                            nDocumentDetails.PostalCode = docPostalCode;

                                                        nDocumentDetails.DocumentName = System.IO.Path.GetFileName(FileAbsoluteUri);
                                                    nDocumentDetails.DocumentURL = FileAbsoluteUri;
                                                        if (jsonString.Contains("DOB"))
                                                        {
                                                            if (string.IsNullOrEmpty(docinfo.details.DOB.ToString()) )
                                                            {


                                                                nDocumentDetails.DOB = DateTime.Parse(docinfo.details.DOB.ToString(), invariantCulture);
                                                            }
                                                            else { strReason += string.Format("{0}DOB is missing", Environment.NewLine); }
                                                        }
                                                    if (docJSONIdentifier != null && docJSONIdentifier != string.Empty)
                                                    {
                                                            // strJSONIdentifier = docinfo[0].docProps["doc.type"].Value.ToString();
                                                          
                                                        // strJSONIdentifier = string()
                                                        nDocumentDetails.JSONFileIdentifier = docJSONIdentifier;
                                                            docJSONIdentifier = docJSONIdentifier.ToLowerInvariant();

                                                        var CategoryNameExists = db.DocumentIdentifiers.Any(x => x.JSONIdentifier.Equals(docJSONIdentifier));
                                                        if (CategoryNameExists)
                                                        {
                                                            var strCategoryName = string.Empty;
                                                            strCategoryName = db.DocumentIdentifiers.FirstOrDefault(x => x.JSONIdentifier.Equals(docJSONIdentifier)).Category;
                                                           

                                                            nDocumentDetails.Category = strCategoryName;
                                                        }
                                                        else { nDocumentDetails.Category = "Others"; }
                                                       


                                                    }
                                                        if (docSubcategory != null && docSubcategory != string.Empty)
                                                        {

                                                            nDocumentDetails.AssetJSONIdentifier = docSubcategory;
                                                            docSubcategory = docSubcategory.ToLowerInvariant();

                                                            var SubCategoryNameExists = db.SubCategoryJSONIdentifiers.Any(x => x.SCJSONIdentifier.Equals(docSubcategory));
                                                            if (SubCategoryNameExists)
                                                            {
                                                                var strSubCategoryName = string.Empty;
                                                                strSubCategoryName = db.SubCategoryJSONIdentifiers.FirstOrDefault(x => x.SCJSONIdentifier.Equals(docSubcategory)).SubCategoryName;


                                                                nDocumentDetails.SubCategory = strSubCategoryName;
                                                            }
                                                            else { nDocumentDetails.SubCategory = "Others"; }



                                                        }
                                                      
                                                           
                                                    if (docOrganisation !=null && docOrganisation !=string.Empty)
                                                    nDocumentDetails.Institution = docOrganisation;
                                                        if(docAccountNo!=null && docAccountNo!=string.Empty)
                                                        nDocumentDetails.AccountNo = docAccountNo;
                                                    nDocumentDetails.FileStatus = "Green";
                                                        if (strReason != null && strReason != string.Empty)
                                                            nDocumentDetails.Reason = strReason;
                                                    nDocumentDetails.IsActive = true;
                                                    nDocumentDetails.CreatedBy = userId.ToString();
                                                    nDocumentDetails.CreatedOn = DateTime.Now;
                                                    iCustomerID = db.CustomerDetails.FirstOrDefault(m => m.CustomerFirstName.Equals(strCustomerFirstName) && m.CustomerMiddleName.Equals(strCustomerMiddleName) && m.CustomerLastName.Equals(strCustomerLastName)).CustomerID;
                                                    nDocumentDetails.CustomerID = iCustomerID;
                                                    db.DocumentDetails.Add(nDocumentDetails);

                                                    db.SaveChanges();
                                                }
                                                else
                                                {
                                                    strErrorMessage += string.Format("This {0} Document is already exists", System.IO.Path.GetFileName(FileAbsoluteUri));
                                                        // ViewData["ErrorMessage"] =            
                                                        //  return View();
                                                        FileAbsoluteUri = string.Empty;
                                                        


                                                }
                                                    strErrorMessage = string.Empty;
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
                                bool isCheckCompliance = docitem.IsCheckCompliance;
                                //  if (string.IsNullOrEmpty(docitem.Reason) && docitem.JSONFileIdentifier != "Client Relationship Agreement")
                                if (isCheckCompliance == false)
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
                                        //else { strComplianceData += string.Format("SSN is missing;{0}", Environment.NewLine); }
                                    }
                                    else { strComplianceData += string.Format("CRA SSN is missing;{0}", Environment.NewLine); }

                                    if (CRADOB.HasValue)
                                    {
                                        if (docDOB.HasValue)
                                        {
                                            if (CRADOB != docDOB)
                                            { strComplianceData += string.Format("DOB is mismatch;{0}", Environment.NewLine); }
                                        }
                                        //else { strComplianceData += string.Format("DOB is missing; {0}", Environment.NewLine); }
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
                                         //   else { strComplianceData += string.Format("Address is missing;{0}", Environment.NewLine); docitem.FileStatus = "Red"; }
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
                                       //     else { strComplianceData += string.Format("Address is missing;{0}", Environment.NewLine); docitem.FileStatus = "Yellow"; }
                                        }
                                        else { strComplianceData += string.Format("CRA Address is missing;{0}", Environment.NewLine); docitem.FileStatus = "Yellow"; }
                                        
                                    }
                                    
                                    docitem.Reason = strComplianceData;

                                    docitem.ModifiedOn = DateTime.Now;
                                    docitem.ModifiedBy = docitem.UserID.ToString();
                                    docitem.IsCheckCompliance = true;
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
        public ActionResult DocumentDetails(int? queryid, int? pID)
        {
            ProfileModel ProfileModel = new ProfileModel();
            if (Session["UserName"] != null && Session["Role"] != null && Session["UserEmailID"] != null)
            {
                string strUserEmailID = Session["UserEmailID"].ToString();
                //   int userId = 0;
                int SessionPortFolioID;
                if(pID != null)
                {
                    Session["dPortFolioID"] = pID;
                }
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
                //Session["SuccessMsg"]= string.Empty;
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

                      //  Session["SuccessMsg"] = string.Format("{0} added successfully...", portfolioname);
                       
                    }
                    return RedirectToAction("PortfolioDetails");
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

        public ActionResult Compliance(int? pID)
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
                        if(pID!=null)
                        {
                            Session["dPortFolioID"] = pID;
                           


                        }
                        if (Session["dPortFolioID"] != null)
                        {
                            SessionPortFolioID = Convert.ToInt32(Session["dPortFolioID"].ToString());
                        }
                        else { SessionPortFolioID = 0; }
                        if (SessionPortFolioID == 0)
                        {
                            var needsVerification = db.DocumentDetails.Where(x => x.FileStatus.Equals("Yellow") && x.IsCheckCompliance== true).Count();
                            var needsAttention = db.DocumentDetails.Where(x => x.FileStatus.Equals("Red") && x.IsCheckCompliance == true).Count();
                            var meetCompliance = db.DocumentDetails.Where(x => x.IsCheckCompliance == true).Count();
                            ViewData["NeedAttention"] = needsAttention;
                            ViewData["NeedVerification"] = needsVerification;
                            ViewData["MeetCompliance"] = meetCompliance;
                             if (meetCompliance > 0)
                            {
                                var dividend = needsAttention + needsVerification + meetCompliance;
                                var complianceScore = (Convert.ToDecimal(meetCompliance) / Convert.ToDecimal(dividend));
                                ViewData["ComplianceScore"] = Convert.ToInt32(complianceScore);
                            }
                            else { ViewData["ComplianceScore"] = 0; }

                            ProfileModel = new ProfileModel
                            {

                                CustomerDetails = db.CustomerDetails.ToList(),
                                PortfolioDetails = db.PortfolioDetails.ToList(),
                                DocumentDetails = db.DocumentDetails.Where(x => x.IsCheckCompliance == true).ToList(),

                                CategoryDetails = db.CategoryDetails.ToList(),
                                SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                            };
                        }
                        else
                        {
                            var needsAttention = db.DocumentDetails.Where(x => x.FileStatus.Equals("Red") && x.IsCheckCompliance == true && x.PortfolioID.Equals(SessionPortFolioID)).Count();
                            var needsVerification = db.DocumentDetails.Where(a => a.FileStatus.Equals("Yellow") && a.IsCheckCompliance == true && a.PortfolioID.Equals(SessionPortFolioID)).Count();
                            var meetCompliance = db.DocumentDetails.Where(m => m.PortfolioID.Equals(SessionPortFolioID) && m.IsCheckCompliance == true).Count();
                            ViewData["NeedAttention"] = needsAttention;

                            ViewData["NeedVerification"] = needsVerification;
                            ViewData["MeetCompliance"] = meetCompliance; if (meetCompliance > 0)
                            {
                                var dividend = needsAttention + needsVerification + meetCompliance;
                                var complianceScore = (Convert.ToDecimal(meetCompliance) / Convert.ToDecimal(dividend));
                                ViewData["ComplianceScore"] = Convert.ToInt32(complianceScore);
                            }
                            else { ViewData["ComplianceScore"] = 0; }

                            ProfileModel = new ProfileModel
                            {

                                CustomerDetails = db.CustomerDetails.Where(x => x.PortfolioID == SessionPortFolioID).ToList(),
                                PortfolioDetails = db.PortfolioDetails.ToList(),
                                DocumentDetails = db.DocumentDetails.Where(x => x.PortfolioID == SessionPortFolioID && x.IsCheckCompliance == true).ToList(),

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
                            var needsVerification = db.DocumentDetails.Where(x => x.FileStatus.Equals("Yellow") && x.IsCheckCompliance ==true).Count();
                            var needsAttention = db.DocumentDetails.Where(x => x.FileStatus.Equals("Red") && x.IsCheckCompliance == true).Count();
                            var meetCompliance = db.DocumentDetails.Where(x =>x.IsCheckCompliance ==true).Count();
                            ViewData["NeedAttention"] = needsAttention;
                            ViewData["NeedVerification"] = needsVerification;
                            ViewData["MeetCompliance"] = meetCompliance;
                            if (meetCompliance > 0)
                            {
                                var dividend = needsAttention + needsVerification + meetCompliance;
                                var complianceScore = (Convert.ToDecimal(meetCompliance) / Convert.ToDecimal(dividend));
                                ViewData["ComplianceScore"] = Convert.ToInt32(complianceScore);
                            }
                            else { ViewData["ComplianceScore"] = 0; }
                            ProfileModel = new ProfileModel
                            {

                                CustomerDetails = db.CustomerDetails.ToList(),
                                PortfolioDetails = db.PortfolioDetails.ToList(),
                                DocumentDetails = db.DocumentDetails.Where(x => x.FileStatus.Equals("Red") && x.IsCheckCompliance==true).ToList(),

                                CategoryDetails = db.CategoryDetails.ToList(),
                                SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                            };
                        }
                        else
                        {

                            var needsAttention = db.DocumentDetails.Where(x => x.FileStatus.Equals("Red") && x.IsCheckCompliance == true && x.PortfolioID.Equals(SessionPortFolioID)).Count();
                            var needsVerification = db.DocumentDetails.Where(a => a.FileStatus.Equals("Yellow") && a.IsCheckCompliance == true && a.PortfolioID.Equals(SessionPortFolioID)).Count();
                            var meetCompliance = db.DocumentDetails.Where(m => m.PortfolioID.Equals(SessionPortFolioID) && m.IsCheckCompliance == true).Count();
                            ViewData["NeedAttention"] = needsAttention;

                            ViewData["NeedVerification"] = needsVerification;
                            ViewData["MeetCompliance"] = meetCompliance; if (meetCompliance > 0)
                            {
                                var dividend = needsAttention + needsVerification + meetCompliance;
                                var complianceScore = (Convert.ToDecimal(meetCompliance) / Convert.ToDecimal(dividend));
                                ViewData["ComplianceScore"] = Convert.ToInt32(complianceScore);
                            }
                            else { ViewData["ComplianceScore"] = 0; }
                            ProfileModel = new ProfileModel
                            {

                                CustomerDetails = db.CustomerDetails.Where(x => x.PortfolioID == SessionPortFolioID).ToList(),
                                PortfolioDetails = db.PortfolioDetails.ToList(),
                                DocumentDetails = db.DocumentDetails.Where(x => x.PortfolioID == SessionPortFolioID && x.IsCheckCompliance == true && x.FileStatus.Equals("Red")).ToList(),

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
                            var needsVerification = db.DocumentDetails.Where(x => x.FileStatus.Equals("Yellow") && x.IsCheckCompliance == true).Count();
                            var needsAttention = db.DocumentDetails.Where(x => x.FileStatus.Equals("Red") && x.IsCheckCompliance == true).Count();
                            var meetCompliance = db.DocumentDetails.Where(x=> x.IsCheckCompliance == true).Count();
                            ViewData["NeedAttention"] = needsAttention;
                            ViewData["NeedVerification"] = needsVerification;
                            ViewData["MeetCompliance"] = meetCompliance;
                            if (meetCompliance > 0)
                            {
                                var dividend = needsAttention + needsVerification + meetCompliance;
                                var complianceScore = (Convert.ToDecimal(meetCompliance) / Convert.ToDecimal(dividend));
                                ViewData["ComplianceScore"] = Convert.ToInt32(complianceScore);
                            }
                            else { ViewData["ComplianceScore"] = 0; }
                            ProfileModel = new ProfileModel
                            {

                                CustomerDetails = db.CustomerDetails.ToList(),
                                PortfolioDetails = db.PortfolioDetails.ToList(),
                                DocumentDetails = db.DocumentDetails.Where(x => x.FileStatus.Equals("Yellow") && x.IsCheckCompliance == true).ToList(),

                                CategoryDetails = db.CategoryDetails.ToList(),
                                SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                            };
                        }
                        else
                        {
                            var needsAttention = db.DocumentDetails.Where(x => x.FileStatus.Equals("Red") && x.IsCheckCompliance == true && x.PortfolioID.Equals(SessionPortFolioID)).Count();
                            var needsVerification = db.DocumentDetails.Where(a => a.FileStatus.Equals("Yellow") && a.IsCheckCompliance == true && a.PortfolioID.Equals(SessionPortFolioID)).Count();
                            var meetCompliance = db.DocumentDetails.Where(m => m.PortfolioID.Equals(SessionPortFolioID) && m.IsCheckCompliance == true).Count();
                            ViewData["NeedAttention"] = needsAttention;

                            ViewData["NeedVerification"] = needsVerification;
                            ViewData["MeetCompliance"] = meetCompliance; if (meetCompliance > 0)
                            {
                                var dividend = needsAttention + needsVerification + meetCompliance;
                                var complianceScore = (Convert.ToDecimal(meetCompliance) / Convert.ToDecimal(dividend));
                                ViewData["ComplianceScore"] = Convert.ToInt32(complianceScore);
                            }
                            else { ViewData["ComplianceScore"] = 0; }
                            ProfileModel = new ProfileModel
                            {

                                CustomerDetails = db.CustomerDetails.Where(x => x.PortfolioID == SessionPortFolioID).ToList(),
                                PortfolioDetails = db.PortfolioDetails.ToList(),
                                DocumentDetails = db.DocumentDetails.Where(x => x.PortfolioID == SessionPortFolioID && x.IsCheckCompliance==true  && x.FileStatus.Equals("Yellow")).ToList(),

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
                            var needsVerification = db.DocumentDetails.Where(x => x.FileStatus.Equals("Yellow") && x.IsCheckCompliance ==true).Count();
                            var needsAttention = db.DocumentDetails.Where(x => x.FileStatus.Equals("Red") && x.IsCheckCompliance == true).Count();
                            var meetCompliance = db.DocumentDetails.Where(x=> x.IsCheckCompliance == true).Count();
                            ViewData["NeedAttention"] = needsAttention;
                            ViewData["NeedVerification"] = needsVerification;
                            ViewData["MeetCompliance"] = meetCompliance;
                            if (meetCompliance > 0)
                            {
                                var dividend = needsAttention + needsVerification + meetCompliance;
                                var complianceScore = (Convert.ToDecimal(meetCompliance) / Convert.ToDecimal(dividend));
                                ViewData["ComplianceScore"] = Convert.ToInt32(complianceScore);
                            }
                            else { ViewData["ComplianceScore"] = 0; }
                            profileModel = new ProfileModel
                            {

                                CustomerDetails = db.CustomerDetails.ToList(),
                                PortfolioDetails = db.PortfolioDetails.ToList(),
                                DocumentDetails = db.DocumentDetails.Where(x => x.IsCheckCompliance == true).ToList(),
                                ddCategoryCompliance = db.DocumentDetails.ToList(),
                                ddInstitutionCompliance = db.DocumentDetails.ToList(),
                                CategoryDetails = db.CategoryDetails.ToList()
                            };


                            if (!string.IsNullOrEmpty(SessionCategory))
                                profileModel.ddCategoryCompliance = db.DocumentDetails.Where(x => x.Category.Equals(SessionCategory) && x.IsCheckCompliance ==true).ToList();

                            if (!string.IsNullOrEmpty(SessionInstitution))
                                profileModel.ddInstitutionCompliance = db.DocumentDetails.Where(x => x.Institution.Equals(SessionInstitution) && x.IsCheckCompliance == true).ToList();

                        }
                        else
                        {
                            var needsAttention= db.DocumentDetails.Where(x => x.FileStatus.Equals("Red") && x.IsCheckCompliance ==true && x.PortfolioID.Equals(SessionPortFolioID)).Count();
                            var needsVerification = db.DocumentDetails.Where(a => a.FileStatus.Equals("Yellow") && a.IsCheckCompliance == true && a.PortfolioID.Equals(SessionPortFolioID)).Count();
                            var meetCompliance = db.DocumentDetails.Where(m => m.PortfolioID.Equals(SessionPortFolioID) && m.IsCheckCompliance == true).Count();
                            ViewData["NeedAttention"] = needsAttention;
                           
                            ViewData["NeedVerification"] = needsVerification;
                            ViewData["MeetCompliance"] = meetCompliance; if (meetCompliance > 0)
                            {
                                var dividend = needsAttention + needsVerification + meetCompliance;
                                var complianceScore = (Convert.ToDecimal(meetCompliance) / Convert.ToDecimal(dividend));
                                ViewData["ComplianceScore"] = Convert.ToInt32(complianceScore);
                            }
                            else { ViewData["ComplianceScore"] = 0; }

                            profileModel = new ProfileModel
                            {

                                CustomerDetails = db.CustomerDetails.Where(x => x.PortfolioID == SessionPortFolioID).ToList(),
                                PortfolioDetails = db.PortfolioDetails.ToList(),
                                ddInstitutionCompliance = db.DocumentDetails.Where(x => x.PortfolioID == SessionPortFolioID && x.IsCheckCompliance==true).ToList(),
                                ddCategoryCompliance = db.DocumentDetails.Where(x => x.PortfolioID == SessionPortFolioID && x.IsCheckCompliance == true).ToList(),
                                CategoryDetails = db.CategoryDetails.ToList(),
                                SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                                DocumentDetails = db.DocumentDetails.Where(x => x.PortfolioID == SessionPortFolioID && x.IsCheckCompliance == true).ToList(),
                            };

                            if (!string.IsNullOrEmpty(SessionCategory))
                                profileModel.ddCategoryCompliance = db.DocumentDetails.Where(x => x.PortfolioID == SessionPortFolioID && x.IsCheckCompliance == true && x.Category.Equals(SessionCategory)).ToList();

                            if (!string.IsNullOrEmpty(SessionInstitution))
                                profileModel.ddInstitutionCompliance = db.DocumentDetails.Where(x => x.PortfolioID == SessionPortFolioID && x.IsCheckCompliance == true && x.Institution.Equals(SessionInstitution)).ToList();

                        }
                    }
                }
                return View(profileModel);
            }
            else { return RedirectToAction("Login", "Login"); }

        }

        public ActionResult Home()
        {
            ProfileModel ProfileModel = new ProfileModel();
            if (Session["UserName"] != null && Session["Role"] != null && Session["UserEmailID"] != null)
            {
                // string strUserEmailID = Session["UserEmailID"].ToString();
                using (docu3cEntities db = new docu3cEntities())
                {
                    ProfileModel = new ProfileModel
                    {

                        CustomerDetails = db.CustomerDetails.ToList(),
                        
                        DocumentDetails = db.DocumentDetails.ToList(),
                        ddCategoryCompliance = db.DocumentDetails.ToList(),
                        ddInstitutionCompliance = db.DocumentDetails.ToList(),
                        CategoryDetails = db.CategoryDetails.ToList(),
                        PortfolioDetails = db.PortfolioDetails.ToList(),
                      
                       
                    };
                  
                }

                return View(ProfileModel);
            }
           
              else { return RedirectToAction("Login", "Login"); }
        }
    }

}
