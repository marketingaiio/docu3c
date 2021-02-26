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


                        DisplayAgreegateStripCount(SessionPortFolioID);

                        if (SessionPortFolioID == 0)
                        {
                           
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
                                Assets =db.SubCategoryJSONIdentifiers.ToList()


                            };


                           

                        }
                        else
                        {
                          
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
                                                string SERVICE_URL = "http://signed-modelservice.azurewebsites.net/?url=";
                                                //  docu3cAPIClient d3 = new docu3cAPIClient();
                                                string fileurl = "https://docu3capp.blob.core.windows.net/uploadfiles/";
                                                HttpClient client = new HttpClient();
                                                client.BaseAddress = new Uri(SERVICE_URL + FileAbsoluteUri);
                                                var request = new HttpRequestMessage(HttpMethod.Post, string.Empty);

                                                var response = client.SendAsync(request).Result;
                                               

                                                var jsonString = response.Content.ReadAsStringAsync().Result;
                                                if (jsonString.Contains("Category"))
                                                {
                                                    //  jsonString = Regex.Replace(jsonString, @"(""[^""\\]*(?:\\.[^""\\]*)*"")|\s+", "$1");
                                                    var docinfo = JsonConvert.DeserializeObject<docu3cAPI>(jsonString);
                                                
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
                                                    if (docinfo.details.FirstName != null && docinfo.details.FirstName != string.Empty)
                                                        strCustomerFirstName = docinfo.details.FirstName.ToString();

                                                    if (docinfo.details.MiddleName != null && docinfo.details.MiddleName != string.Empty)
                                                        strCustomerMiddleName = docinfo.details.MiddleName.ToString();
                                                    if (docinfo.details.LastName != null && docinfo.details.LastName != string.Empty)
                                                        strCustomerLastName = docinfo.details.LastName.ToString();
                                                    if (!string.IsNullOrEmpty(strCustomerFirstName) && !string.IsNullOrEmpty(strCustomerLastName))
                                                    {
                                                        strCustomerName = string.Format("{0} {1} {2}", strCustomerFirstName, strCustomerMiddleName, strCustomerLastName);
                                                    }
                                                    else { strReason = "Name is missing; "; }


                                                        ///Client2 
                                                        ///

                                                        string strClient2FirstName = string.Empty;string strClient2MiddleName = string.Empty;
                                                        string strClient2LastName = string.Empty; string strClient2Name = string.Empty;
                                                        if (docinfo.details.Client2FirstName  != null && docinfo.details.Client2FirstName != string.Empty)
                                                            strClient2FirstName = docinfo.details.Client2FirstName.ToString();

                                                        if (docinfo.details.Client2MiddleName != null && docinfo.details.Client2MiddleName != string.Empty)
                                                            strClient2MiddleName = docinfo.details.Client2MiddleName.ToString();
                                                        if (docinfo.details.Client2LastName != null && docinfo.details.Client2LastName != string.Empty)
                                                            strClient2LastName = docinfo.details.Client2LastName.ToString();
                                                        if (!string.IsNullOrEmpty(strClient2FirstName) && !string.IsNullOrEmpty(strClient2LastName))
                                                        {
                                                            strClient2Name = string.Format("{0} {1} {2}", strClient2FirstName, strClient2MiddleName, strClient2LastName);
                                                        }

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
                                                        string docAccountType = string.Empty;

                                                    string[] formats = { "dd.MM.yyyy", "dd-MM-yyyy", "dd/MM/yyyy" };
                                                    DateTime dtDOB;
                                                    nDocumentDetails.ClassifyJSON = jsonString;
                                                    if (docinfo.details.Street != null && docinfo.details.Street != string.Empty)

                                                        docAddress = docinfo.details.Street.ToString();


                                                        if (docinfo.details.AccountType != null && docinfo.details.AccountType != string.Empty)
                                                            docAccountType = docinfo.details.AccountType.ToString();
                                                            if (docinfo.details.City != null && docinfo.details.City != string.Empty)
                                                        docCity = docinfo.details.City.ToString();
                                                    if (docinfo.details.State != null && docinfo.details.State != string.Empty)
                                                        docState = docinfo.details.State.ToString();
                                                    if (docinfo.details.Zipcode != null && docinfo.details.Zipcode != string.Empty)
                                                        docPostalCode = docinfo.details.Zipcode.ToString();
                                                    if (string.IsNullOrEmpty(docAddress) || string.IsNullOrEmpty(docCity) || string.IsNullOrEmpty(docState) || string.IsNullOrEmpty(docPostalCode))
                                                    { strReason += string.Format(" {0}Address is missing; ", Environment.NewLine); }

                                                    if (docinfo.details.Organization != null && docinfo.details.Organization != string.Empty)
                                                    {
                                                        docOrganisation = docinfo.details.Organization.ToString();
                                                    }
                                                    else { strReason += string.Format(" {0}Institution is missing; ", Environment.NewLine); }
                                                    if (docinfo.details.SSN != null && docinfo.details.SSN != string.Empty)
                                                    {
                                                        docSSN = docinfo.details.SSN.ToString();
                                                    }
                                                    else { strReason += string.Format(" {0}SSN is missing; ", Environment.NewLine); }
                                                    if (docinfo.details.Category != null && docinfo.details.Category != string.Empty)
                                                        docJSONIdentifier = docinfo.details.Category.ToString();
                                                    if (docinfo.SubCategory != null && docinfo.SubCategory != string.Empty)
                                                    {
                                                        docSubcategory = docinfo.SubCategory.ToString();
                                                    }

                                                    if (docinfo.details.AccountNo != null && docinfo.details.AccountNo != string.Empty)
                                                    {
                                                        docAccountNo = docinfo.details.AccountNo.ToString();
                                                    }
                                                    else { strReason += string.Format(" {0}Account No. is missing; ", Environment.NewLine); }


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

                                                                if (docinfo.details.DOB != null && docinfo.details.DOB != string.Empty)
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
                                                            exCustomerDetails = db.CustomerDetails.Where(m => m.CustomerFirstName.Equals(strCustomerFirstName) && m.CustomerMiddleName.Equals(strCustomerMiddleName) && m.CustomerLastName.Equals(strCustomerLastName)).ToList();
                                                            foreach (var item in exCustomerDetails)
                                                            {

                                                                //  strJSONIdentifier = strJSONIdentifier.Replace("_", " ");

                                                                if (docJSONIdentifier == "Client Relationship Agreement")
                                                                {

                                                                        if (jsonString.Contains("DOB"))
                                                                        {
                                                                            if (docinfo.details.DOB != null && docinfo.details.DOB != string.Empty)
                                                                            {


                                                                                item.DOB = DateTime.Parse(docinfo.details.DOB.ToString(), invariantCulture);
                                                                            }
                                                                            else { strReason += string.Format(" {0}DOB is missing; ", Environment.NewLine); }
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

                                                            if (docJSONIdentifier == "Client Relationship Agreement")
                                                            {
                                                                nDocumentDetails.IsCheckCompliance = true;
                                                            }

                                                                if (!string.IsNullOrEmpty(docSSN))
                                                            nDocumentDetails.DocCustomerID = docSSN;
                                                        nDocumentDetails.PortfolioID = iPortFolioID;
                                                        nDocumentDetails.UserID = userId;
                                                        if (!string.IsNullOrEmpty(strCustomerName))
                                                            nDocumentDetails.CustomerName = strCustomerName;

                                                        if (!string.IsNullOrEmpty(docAddress))
                                                            nDocumentDetails.Address = docAddress;
                                                        if (!string.IsNullOrEmpty(docCity))
                                                            nDocumentDetails.City = docCity;
                                                        if (!string.IsNullOrEmpty(docState))
                                                            nDocumentDetails.State = docState;
                                                        if (!string.IsNullOrEmpty(docPostalCode))
                                                            nDocumentDetails.PostalCode = docPostalCode;

                                                        nDocumentDetails.DocumentName = System.IO.Path.GetFileName(FileAbsoluteUri);
                                                        nDocumentDetails.DocumentURL = FileAbsoluteUri;
                                                        if (jsonString.Contains("DOB"))
                                                        {
                                                            if (docinfo.details.DOB != null && docinfo.details.DOB != string.Empty)
                                                            {


                                                                nDocumentDetails.DOB = DateTime.Parse(docinfo.details.DOB.ToString(), invariantCulture);
                                                            }
                                                            else { strReason += string.Format(" {0}DOB is missing; ", Environment.NewLine); }
                                                        }
                                                        if (docJSONIdentifier != null && docJSONIdentifier != string.Empty)
                                                        {
                                                                // strJSONIdentifier = docinfo[0].docProps["doc.type"].Value.ToString();

                                                                // strJSONIdentifier = string()
                                                                docJSONIdentifier = docJSONIdentifier.Replace("_", " ");
                                                                docJSONIdentifier = docJSONIdentifier.Replace("®", " ");
                                                                docJSONIdentifier = docJSONIdentifier.Replace("*", "");
                                                                nDocumentDetails.JSONFileIdentifier = docJSONIdentifier;
                                                            docJSONIdentifier = docJSONIdentifier.ToLowerInvariant();

                                                            var CategoryNameExists = db.DocumentIdentifiers.Any(x => x.JSONIdentifier.Contains(docJSONIdentifier));
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
                                                            else {
                                                                    if (docJSONIdentifier != "Not Present")
                                                                  
                                                                        nDocumentDetails.SubCategory = "Others";
                                                                    
                                                                    strReason += string.Format(" {0}Asset is missing; ", Environment.NewLine); 
                                                                }



                                                        }


                                                        if (docOrganisation != null && docOrganisation != string.Empty)
                                                            nDocumentDetails.Institution = docOrganisation;
                                                        if (docAccountNo != null && docAccountNo != string.Empty)
                                                            nDocumentDetails.AccountNo = docAccountNo;
                                                        nDocumentDetails.FileStatus = "Green";
                                                        if (!string.IsNullOrEmpty(strReason))
                                                            nDocumentDetails.Reason = strReason;
                                                        nDocumentDetails.IsActive = true;
                                                        nDocumentDetails.CreatedBy = userId.ToString();
                                                        nDocumentDetails.CreatedOn = DateTime.Now;
                                                        iCustomerID = db.CustomerDetails.FirstOrDefault(m => m.CustomerFirstName.Equals(strCustomerFirstName) && m.CustomerMiddleName.Equals(strCustomerMiddleName) && m.CustomerLastName.Equals(strCustomerLastName)).CustomerID;
                                                        nDocumentDetails.CustomerID = iCustomerID;
                                                        nDocumentDetails.AccountType = docAccountType;
                                                            if(docAccountType != "Single")
                                                            {
                                                                nDocumentDetails.Client2Name = strClient2Name;

                                                            }
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
                                                else { strErrorMessage += "Document not recognized"; }
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
                int iSessionCustomerID = Convert.ToInt32(Session["CustomerID"].ToString());
                if (iSessionCustomerID >0)
                {
                    List<DocumentDetail> documentDetails = new List<DocumentDetail>();
                    using (docu3cEntities db = new docu3cEntities())
                        {
                        string CustomerCRAJSON = string.Empty;
                        int iCustomerID = Convert.ToInt32(iSessionCustomerID);
                        documentDetails = db.DocumentDetails.Where(m => m.CustomerID.Equals(iCustomerID) && m.JSONFileIdentifier != "Client Relationship Agreement").ToList();
                        if (db.DocumentDetails.Where(m => m.JSONFileIdentifier == "Client Relationship Agreement" && m.CustomerID.Equals(iCustomerID)).Count() > 0)
                        {
                            CustomerCRAJSON = db.DocumentDetails.Where(m => m.JSONFileIdentifier == "Client Relationship Agreement" && m.CustomerID.Equals(iCustomerID)).FirstOrDefault().ClassifyJSON;
                            if (!string.IsNullOrEmpty(CustomerCRAJSON))
                            {
                                foreach (var docitem in documentDetails)
                                {
                                    string strComplianceData = string.Empty;
                                    bool isCheckCompliance = docitem.IsCheckCompliance;
                                    string strDocumentClassifyJSON = docitem.ClassifyJSON;
                                    if (docitem.JSONFileIdentifier != "Client Relationship Agreement" && isCheckCompliance == false)
                                    {

                                        var ComplianceServiceURL = string.Format("{0}{1}{2}{3}", "https://compliance-service123.azurewebsites.net/?json1=", CustomerCRAJSON, "&json2=", strDocumentClassifyJSON);
                                        HttpClient client = new HttpClient();
                                        client.BaseAddress = new Uri(ComplianceServiceURL);
                                        var request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                                        string strComplianceReason = string.Empty;
                                        var response = client.SendAsync(request).Result;


                                        var jsonString = response.Content.ReadAsStringAsync().Result;
                                        var docComplianceinfo = JsonConvert.DeserializeObject<DocumentController>(jsonString);
                                        if (docComplianceinfo != null)
                                        {
                                            docitem.ComplianceJSONOutput = jsonString;
                                            if (!string.IsNullOrEmpty(docComplianceinfo.FullName.ToString()) && docComplianceinfo.FullName.ToString() != "---")

                                                docitem.IsCustomerName = docComplianceinfo.FullName.ToString();

                                            if (docComplianceinfo.FullName.ToString() == "Mismatched")
                                            { strComplianceReason += "Customer Name: Mismatched; "; }


                                            if (!string.IsNullOrEmpty(docComplianceinfo.DOB.ToString()) && docComplianceinfo.DOB.ToString() != "---")
                                                docitem.IsDOB = docComplianceinfo.DOB.ToString();
                                            if (docComplianceinfo.DOB.ToString() == "Mismatched")
                                            { strComplianceReason += " DOB: Mismatched; "; }


                                            if (!string.IsNullOrEmpty(docComplianceinfo.Address.ToString()) && docComplianceinfo.Address.ToString() != "---")
                                                docitem.IsAddress = docComplianceinfo.Address.ToString();
                                            if (docComplianceinfo.Address.ToString() == "Mismatched")
                                            { strComplianceReason += " Address: Mismatched; "; }

                                            if (!string.IsNullOrEmpty(docComplianceinfo.AccountNo.ToString()) && docComplianceinfo.AccountNo.ToString() != "---")
                                                docitem.IsAccountNo = docComplianceinfo.AccountNo.ToString();
                                            if (docComplianceinfo.AccountNo.ToString() == "Mismatched")
                                            { strComplianceReason += " Account No: Mismatched; "; }

                                            if (!string.IsNullOrEmpty(docComplianceinfo.SSN.ToString()) && docComplianceinfo.SSN.ToString() != "---")
                                                docitem.IsSSN = docComplianceinfo.SSN.ToString();
                                            if (docComplianceinfo.SSN.ToString() == "Mismatched")
                                            { strComplianceReason += " SSN: Mismatched; "; }

                                            if (!string.IsNullOrEmpty(docComplianceinfo.SubCategory.ToString()) && docComplianceinfo.SubCategory.ToString() != "---")
                                                docitem.IsSubCategory = docComplianceinfo.SubCategory.ToString();
                                            if (docComplianceinfo.SubCategory.ToString() == "Mismatched")
                                            { strComplianceReason += " SubCategory: Mismatched; "; }

                                            if (!string.IsNullOrEmpty(docComplianceinfo.Signature.ToString()) && docComplianceinfo.Signature.ToString() != "---")
                                                docitem.IsSignature = docComplianceinfo.Signature.ToString();
                                            if (docComplianceinfo.Signature.ToString() == "Mismatched")
                                            { strComplianceReason += " Signature: Mismatched; "; }

                                            if (!string.IsNullOrEmpty(docComplianceinfo.ViolationofMutualFund.ToString()) && docComplianceinfo.ViolationofMutualFund.ToString() != "---")
                                                docitem.IsMutualViolation = docComplianceinfo.ViolationofMutualFund.ToString();
                                            if (docComplianceinfo.ViolationofMutualFund.ToString() == "Mismatched")
                                            { strComplianceReason += "  Violation of MutualFund: Mismatched; "; }
                                            docitem.IsCheckCompliance = true;
                                            if (!string.IsNullOrEmpty(strComplianceReason))
                                                docitem.ComplianceReason = strComplianceReason;
                                            db.SaveChanges();
                                           
                                        }
                                    }
                                }
                            }
                            else { ViewData["AlertMessage"] = "CRA does not exist"; }
                        }
                        else { ViewData["AlertMessage"] = "CRA does not exist"; return string.Empty; }
                }
                }

            }
            return string.Empty;
        }




        public ActionResult About()
        {

            return View();

        }

        public string DisplayAgreegateStripCount(int? SessionPortFolioID)
        {
            using (docu3cEntities db = new docu3cEntities())
            {
                if (SessionPortFolioID == 0)
                {
                    ViewData["NoofCategory"] = db.DocumentDetails.Select(o => o.Category).Distinct().Count();
                    ViewData["NoofCustomers"] = db.CustomerDetails.Count();
                    ViewData["NoofDocumets"] = db.DocumentDetails.Count();

                    ViewData["NoOfInstitution"] = db.DocumentDetails.Distinct().GroupBy(o => new
                    {
                        o.Institution
                    }).Count();
                }
                else
                {
                    ViewData["NoofCategory"] = db.DocumentDetails.Where(x=>x.PortfolioID == SessionPortFolioID).Select(o => o.Category).Distinct().Count();
                    ViewData["NoofCustomers"] = db.CustomerDetails.Where(x => x.PortfolioID == SessionPortFolioID).Count();
                    ViewData["NoofDocumets"] = db.DocumentDetails.Where(x => x.PortfolioID == SessionPortFolioID).Count();

                    ViewData["NoOfInstitution"] = db.DocumentDetails.Where(x => x.PortfolioID == SessionPortFolioID).Distinct().GroupBy(o => new
                    {
                        o.Institution
                    }).Count();

                }
            }
            return string.Empty;

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
        public ActionResult DocumentDetails(int? queryid,string SC, string qa, int? pID)
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
                        
                        DisplayAgreegateStripCount(SessionPortFolioID);
                        if (SessionPortFolioID == 0)
                        {
                            // ViewData["PortfolioName"] = db.PortfolioDetails.FirstOrDefault(m => m.UserID.Equals(userId)).PortfolioName;

                            if (!string.IsNullOrEmpty(strCategoryName))
                            {
                                if (!string.IsNullOrEmpty(SC) && !string.IsNullOrEmpty(qa))
                                {
                                    // string dSubCategory = db.SubCategoryJSONIdentifiers.FirstOrDefault(m => m.SCID.Equals(SC)).SubCategoryName;
                                    ProfileModel = new ProfileModel
                                    {

                                        CustomerDetails = db.CustomerDetails.ToList(),
                                        PortfolioDetails = db.PortfolioDetails.ToList(),
                                        DocumentDetails = db.DocumentDetails.Include("CustomerDetail").Where(m => m.Category == strCategoryName && m.SubCategory == SC && m.AssetJSONIdentifier == qa).ToList(),

                                        CategoryDetails = db.CategoryDetails.ToList(),
                                        SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                                        Assets = db.SubCategoryJSONIdentifiers.ToList(),
                                    };
                                }
                                else
                                {

                                    ProfileModel = new ProfileModel
                                    {

                                        CustomerDetails = db.CustomerDetails.ToList(),
                                        PortfolioDetails = db.PortfolioDetails.ToList(),
                                        DocumentDetails = db.DocumentDetails.Include("CustomerDetail").Where(m => m.Category == strCategoryName).ToList(),

                                        CategoryDetails = db.CategoryDetails.ToList(),
                                        SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                                        Assets = db.SubCategoryJSONIdentifiers.ToList(),
                                    };
                                }

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
                                    Assets = db.SubCategoryJSONIdentifiers.ToList(),
                                };
                            }
                           
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(strCategoryName))
                            {
                                if (!string.IsNullOrEmpty(SC) && !string.IsNullOrEmpty(qa))
                                {
                                    // string dSubCategory = db.SubCategoryJSONIdentifiers.FirstOrDefault(m => m.SCID.Equals(SC)).SubCategoryName;
                                    ProfileModel = new ProfileModel
                                    {

                                        CustomerDetails = db.CustomerDetails.ToList(),
                                        PortfolioDetails = db.PortfolioDetails.ToList(),
                                        DocumentDetails = db.DocumentDetails.Include("CustomerDetail").Where(m => m.Category == strCategoryName && m.SubCategory == SC && m.AssetJSONIdentifier == qa).ToList(),

                                        CategoryDetails = db.CategoryDetails.ToList(),
                                        SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                                        Assets = db.SubCategoryJSONIdentifiers.ToList(),
                                    };
                                }
                                else
                                {
                                    ProfileModel = new ProfileModel
                                    {

                                        CustomerDetails = db.CustomerDetails.Where(x => x.PortfolioID == SessionPortFolioID).ToList(),
                                        PortfolioDetails = db.PortfolioDetails.ToList(),
                                        DocumentDetails = db.DocumentDetails.Include("CustomerDetail").Where(x => x.PortfolioID == SessionPortFolioID && x.Category == strCategoryName).ToList(),
                                        CategoryDetails = db.CategoryDetails.ToList(),
                                        SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                                        Assets = db.SubCategoryJSONIdentifiers.ToList(),
                                    };
                                }
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
                                    Assets = db.SubCategoryJSONIdentifiers.ToList(),
                                };
                            }
                          
                        }
                    }
                }
                return View(ProfileModel);
            }
            else { return RedirectToAction("Login", "Login"); }

        }
        public ActionResult CustomerDetails(int? queryid, string SC, string qa)
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
                        DisplayAgreegateStripCount(SessionPortFolioID);
                        if (SessionPortFolioID == 0)
                        {

                            if (!string.IsNullOrEmpty(strCategoryName))
                            {
                                if (!string.IsNullOrEmpty(SC) && !string.IsNullOrEmpty(qa))
                                {
                                    ProfileModel = new ProfileModel
                                    {

                                        CustomerDetails = db.CustomerDetails.Where(m => m.DocumentDetails.FirstOrDefault().Category == strCategoryName && m.DocumentDetails.FirstOrDefault().SubCategory == SC && m.DocumentDetails.FirstOrDefault().AssetJSONIdentifier == qa).ToList(),
                                        PortfolioDetails = db.PortfolioDetails.ToList(),
                                        DocumentDetails = db.DocumentDetails.Include("CustomerDetail").ToList(),
                                        CategoryDetails = db.CategoryDetails.ToList(),
                                        SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                                    };
                                }
                                else {
                                    ProfileModel = new ProfileModel
                                    {

                                        CustomerDetails = db.CustomerDetails.Where(m => m.DocumentDetails.FirstOrDefault().Category == strCategoryName).ToList(),
                                        PortfolioDetails = db.PortfolioDetails.ToList(),
                                        DocumentDetails = db.DocumentDetails.Include("CustomerDetail").ToList(),
                                        CategoryDetails = db.CategoryDetails.ToList(),
                                        SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                                    };
                                }
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
                           
                        }
                        else
                        {

                            if (!string.IsNullOrEmpty(strCategoryName))
                            {
                                if (!string.IsNullOrEmpty(SC) && !string.IsNullOrEmpty(qa))
                                {
                                    ProfileModel = new ProfileModel
                                    {

                                        CustomerDetails = db.CustomerDetails.Where(m => m.PortfolioID == SessionPortFolioID && m.DocumentDetails.FirstOrDefault().Category == strCategoryName && m.DocumentDetails.FirstOrDefault().SubCategory == SC && m.DocumentDetails.FirstOrDefault().AssetJSONIdentifier == qa).ToList(),
                                        PortfolioDetails = db.PortfolioDetails.ToList(),
                                        DocumentDetails = db.DocumentDetails.Include("CustomerDetail").Where(x => x.PortfolioID == SessionPortFolioID && x.Category == strCategoryName).ToList(),
                                        CategoryDetails = db.CategoryDetails.ToList(),
                                        SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                                    };
                                }
                                else
                                {
                                    ProfileModel = new ProfileModel
                                    {

                                        CustomerDetails = db.CustomerDetails.Where(m => m.PortfolioID == SessionPortFolioID && m.DocumentDetails.FirstOrDefault().Category == strCategoryName).ToList(),
                                        PortfolioDetails = db.PortfolioDetails.ToList(),
                                        DocumentDetails = db.DocumentDetails.Include("CustomerDetail").Where(x => x.PortfolioID == SessionPortFolioID && x.Category == strCategoryName).ToList(),
                                        CategoryDetails = db.CategoryDetails.ToList(),
                                        SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                                    };
                                }
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
                            

                        }
                    }
                }
                return View(ProfileModel);
            }
            else { return RedirectToAction("Login", "Login"); }

        }

        public ActionResult InstitutionDetails(int? queryid, string SC, string qa)
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
                        DisplayAgreegateStripCount(SessionPortFolioID);
                        if (SessionPortFolioID == 0)
                        {
                            //ViewData["PortfolioName"] = db.PortfolioDetails.FirstOrDefault(m => m.UserID.Equals(userId)).PortfolioName;

                            if (!string.IsNullOrEmpty(strCategoryName))
                            {
                                if (!string.IsNullOrEmpty(SC) && !string.IsNullOrEmpty(qa))
                                {
                                    ProfileModel = new ProfileModel
                                    {

                                        CustomerDetails = db.CustomerDetails.ToList(),
                                        PortfolioDetails = db.PortfolioDetails.ToList(),
                                        DocumentDetails = db.DocumentDetails.Where(m => m.Category == strCategoryName && m.SubCategory == SC && m.AssetJSONIdentifier == qa).ToList(),

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
                                        DocumentDetails = db.DocumentDetails.Where(m => m.Category == strCategoryName).ToList(),

                                        CategoryDetails = db.CategoryDetails.ToList(),
                                        SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                                    };
                                }

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
                           
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(strCategoryName))
                            {

                                if (!string.IsNullOrEmpty(SC) && !string.IsNullOrEmpty(qa))
                                {
                                    ProfileModel = new ProfileModel
                                    {

                                        CustomerDetails = db.CustomerDetails.ToList(),
                                        PortfolioDetails = db.PortfolioDetails.ToList(),
                                        DocumentDetails = db.DocumentDetails.Where(m => m.Category == strCategoryName && m.SubCategory == SC && m.AssetJSONIdentifier == qa).ToList(),

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
                                    DocumentDetails = db.DocumentDetails.Where(x => x.PortfolioID == SessionPortFolioID && x.Category  ==strCategoryName).ToList(),

                                    CategoryDetails = db.CategoryDetails.ToList(),
                                    SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                                };
                                    }
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


        public ActionResult MissingInfo()
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
                           
                            ProfileModel = new ProfileModel
                            {

                                CustomerDetails = db.CustomerDetails.ToList(),
                                PortfolioDetails = db.PortfolioDetails.ToList(),
                                DocumentDetails = db.DocumentDetails.Where(x => x.IsCheckCompliance == true).ToList(),

                                CategoryDetails = db.CategoryDetails.ToList(),
                                SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                            };
                            DisplayComplianceBand(SessionPortFolioID, ProfileModel);
                        }
                        else
                        {
                           
                            ProfileModel = new ProfileModel
                            {

                                CustomerDetails = db.CustomerDetails.Where(x => x.PortfolioID == SessionPortFolioID).ToList(),
                                PortfolioDetails = db.PortfolioDetails.ToList(),
                                DocumentDetails = db.DocumentDetails.Where(x =>  x.PortfolioID == SessionPortFolioID && x.IsCheckCompliance == true).ToList(),

                                CategoryDetails = db.CategoryDetails.ToList(),
                                SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                            };
                            DisplayComplianceBand(SessionPortFolioID, ProfileModel);
                        }
                    }
                }
                return View(ProfileModel);
            }
            else { return RedirectToAction("Login", "Login"); }

        }

        public ActionResult MismatchInfo()
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

                            ProfileModel = new ProfileModel
                            {

                                CustomerDetails = db.CustomerDetails.ToList(),
                                PortfolioDetails = db.PortfolioDetails.ToList(),
                                DocumentDetails = db.DocumentDetails.Where(m => m.IsCheckCompliance == true).ToList(),

                                CategoryDetails = db.CategoryDetails.ToList(),
                                SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                            };
                            DisplayComplianceBand(SessionPortFolioID, ProfileModel);
                        }
                        else
                        {

                            ProfileModel = new ProfileModel
                            {

                                CustomerDetails = db.CustomerDetails.Where(x => x.PortfolioID == SessionPortFolioID).ToList(),
                                PortfolioDetails = db.PortfolioDetails.ToList(),
                                DocumentDetails = db.DocumentDetails.Where(x => x.PortfolioID == SessionPortFolioID && x.IsCheckCompliance == true).ToList(),

                                CategoryDetails = db.CategoryDetails.ToList(),
                                SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                            };
                            DisplayComplianceBand(SessionPortFolioID, ProfileModel);
                        }
                       

                    }
                }
                return View(ProfileModel);
            }
            else { return RedirectToAction("Login", "Login"); }

        }

        public ActionResult ComplianceDashboard()
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
                      
                        int iMissingInfoTotal; 
                        int iMismatchInfoTotal;
                        if (SessionPortFolioID == 0)
                        {

                            ProfileModel = new ProfileModel
                            {

                                CustomerDetails = db.CustomerDetails.ToList(),
                                PortfolioDetails = db.PortfolioDetails.ToList(),
                                DocumentDetails = db.DocumentDetails.Where(x=>x.IsCheckCompliance == true).ToList(),

                                CategoryDetails = db.CategoryDetails.ToList(),
                                SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                            };
                            DisplayComplianceBand(SessionPortFolioID , ProfileModel);
                            if (ProfileModel.DocumentDetails.Where(m => m.Reason != null).Count() > 0)
                            {
                                iMissingInfoTotal = ProfileModel.DocumentDetails.Where(m => !string.IsNullOrEmpty(m.CustomerName)).Count() + ProfileModel.DocumentDetails.Where(m => m.DOB != null).Count()
                                + ProfileModel.DocumentDetails.Where(m => !string.IsNullOrEmpty(m.Address)).Count() +
ProfileModel.DocumentDetails.Where(m => !string.IsNullOrEmpty(m.AccountNo)).Count() +
ProfileModel.DocumentDetails.Where(m => !string.IsNullOrEmpty(m.DocCustomerID)).Count() +
ProfileModel.DocumentDetails.Where(m => !string.IsNullOrEmpty(m.AssetJSONIdentifier)).Count();
                                ViewData["MissingInfoTotal"] = iMissingInfoTotal;
                            }
                            if (ProfileModel.DocumentDetails.Where(m => m.ComplianceJSONOutput != null).Count() > 0)
                            {
                                iMismatchInfoTotal = ProfileModel.DocumentDetails.Where(m => m.IsCustomerName == "Mismatched" && !string.IsNullOrEmpty(m.IsCustomerName)).Count() +
    ProfileModel.DocumentDetails.Where(m => m.IsDOB == "Mismatched" && !string.IsNullOrEmpty(m.IsDOB)).Count() +
    ProfileModel.DocumentDetails.Where(m => m.IsAddress == "Mismatched" && !string.IsNullOrEmpty(m.IsAddress)).Count() +
    ProfileModel.DocumentDetails.Where(m => m.IsAccountNo == "Mismatched" && !string.IsNullOrEmpty(m.IsAccountNo)).Count() +
    ProfileModel.DocumentDetails.Where(m => m.IsSignature == "Mismatched" && !string.IsNullOrEmpty(m.IsSignature)).Count() +
    ProfileModel.DocumentDetails.Where(m => m.IsSSN == "Mismatched" && !string.IsNullOrEmpty(m.IsSSN)).Count() +
    ProfileModel.DocumentDetails.Where(m => m.IsSubCategory == "Mismatched" && !string.IsNullOrEmpty(m.IsSubCategory)).Count() +
    ProfileModel.DocumentDetails.Where(m => m.IsMutualViolation == "Mismatched" && !string.IsNullOrEmpty(m.IsMutualViolation)).Count();
                                ViewData["MismatchInfoTotal"] = iMismatchInfoTotal;
                            }

                        }
                        else
                        {

                            ProfileModel = new ProfileModel
                            {

                                CustomerDetails = db.CustomerDetails.Where(x => x.PortfolioID == SessionPortFolioID).ToList(),
                                PortfolioDetails = db.PortfolioDetails.ToList(),
                                DocumentDetails = db.DocumentDetails.Where(x => x.PortfolioID == SessionPortFolioID && x.IsCheckCompliance == true).ToList(),

                                CategoryDetails = db.CategoryDetails.ToList(),
                                SubCategoryDetails = db.SubCategoryDetails.Include("CategoryDetail").ToList(),
                            };
                            DisplayComplianceBand(SessionPortFolioID, ProfileModel);
                            if (ProfileModel.DocumentDetails.Where(m => m.Reason != null).Count() > 0)
                            {
                                iMissingInfoTotal = ProfileModel.DocumentDetails.Where(m => !string.IsNullOrEmpty(m.CustomerName)).Count() + ProfileModel.DocumentDetails.Where(m => m.DOB != null).Count()
                                + ProfileModel.DocumentDetails.Where(m => !string.IsNullOrEmpty(m.Address)).Count() +
ProfileModel.DocumentDetails.Where(m => !string.IsNullOrEmpty(m.AccountNo)).Count() +
ProfileModel.DocumentDetails.Where(m => !string.IsNullOrEmpty(m.DocCustomerID)).Count() +
ProfileModel.DocumentDetails.Where(m => !string.IsNullOrEmpty(m.AssetJSONIdentifier)).Count();
                                ViewData["MissingInfoTotal"] = iMissingInfoTotal;
                            }
                            if (ProfileModel.DocumentDetails.Where(m => m.ComplianceJSONOutput != null).Count() > 0)
                            {
                                iMismatchInfoTotal = ProfileModel.DocumentDetails.Where(m => m.IsCustomerName == "Mismatched" && !string.IsNullOrEmpty(m.IsCustomerName)).Count() +
    ProfileModel.DocumentDetails.Where(m => m.IsDOB == "Mismatched" && !string.IsNullOrEmpty(m.IsDOB)).Count() +
    ProfileModel.DocumentDetails.Where(m => m.IsAddress == "Mismatched" && !string.IsNullOrEmpty(m.IsAddress)).Count() +
    ProfileModel.DocumentDetails.Where(m => m.IsAccountNo == "Mismatched" && !string.IsNullOrEmpty(m.IsAccountNo)).Count() +
    ProfileModel.DocumentDetails.Where(m => m.IsSignature == "Mismatched" && !string.IsNullOrEmpty(m.IsSignature)).Count() +
    ProfileModel.DocumentDetails.Where(m => m.IsSSN == "Mismatched" && !string.IsNullOrEmpty(m.IsSSN)).Count() +
    ProfileModel.DocumentDetails.Where(m => m.IsSubCategory == "Mismatched" && !string.IsNullOrEmpty(m.IsSubCategory)).Count() +
    ProfileModel.DocumentDetails.Where(m => m.IsMutualViolation == "Mismatched" && !string.IsNullOrEmpty(m.IsMutualViolation)).Count();
                                ViewData["MismatchInfoTotal"] = iMismatchInfoTotal;
                            }
                        }
                        
                        
                    }
                    
                }
                return View(ProfileModel);
            }
            else { return RedirectToAction("Login", "Login"); }

        }


        public string DisplayComplianceBand(int? SessionPortFolioID, ProfileModel ProfileModel)
        {
            using (docu3cEntities db = new docu3cEntities())
            {


                var cMissingInfo = ProfileModel.DocumentDetails.Where(m=>!string.IsNullOrEmpty(m.Reason)).Count();
                var cMismatchInfo = ProfileModel.DocumentDetails.Where(m => !string.IsNullOrEmpty(m.ComplianceReason)).Count();
                int meetCompliance=0;
                if (SessionPortFolioID == 0)
                {
                     meetCompliance = db.DocumentDetails.Where(x => x.IsCheckCompliance == true).Count();
                }
                else {
                    meetCompliance = db.DocumentDetails.Where(x => x.IsCheckCompliance == true && x.PortfolioID== SessionPortFolioID).Count();
                }
                ViewData["bMissingInfo"] = cMissingInfo;
                ViewData["bMismatchInfo"] = cMismatchInfo;
                ViewData["bMeetCompliance"] = meetCompliance;
                if (meetCompliance > 0)
                {
                    var dividend = cMissingInfo + cMismatchInfo + meetCompliance;
                    var complianceScore = (Convert.ToDecimal(dividend) / Convert.ToDecimal(meetCompliance));
                    ViewData["bComplianceScore"] = Convert.ToInt32(complianceScore);
                }
                else { ViewData["bComplianceScore"] = 0; }
            }
            return string.Empty;

        }

        //class end
    }

}
