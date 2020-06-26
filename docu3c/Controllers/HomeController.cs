﻿using docu3c.Models;
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
            return View();
        }

        public ActionResult LogOff()
        {
            Session.Clear();
            return RedirectToAction("Login", "Login");
        }
       

        public ActionResult DocumentsUpload()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DocumentsUpload(HttpPostedFileBase[] files)
        {

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
            return View();
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