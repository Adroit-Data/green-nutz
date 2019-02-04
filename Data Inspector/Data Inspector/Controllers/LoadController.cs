﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text.RegularExpressions;
using Data_Inspector.Models;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System.Threading;
using System.Data.SqlClient;
using System.Configuration;

namespace Data_Inspector.Controllers
{
    public class LoadController : Controller
    {
        
        // GET: Load
        public ActionResult Index()
        {
            return View();
        }

        //
        // POST: Load
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file == null)
                {
                    ModelState.AddModelError("File", "Please Upload Your file");
                }
                else if (file.ContentLength > 0)
                {
                    string[] AllowedFileExtensions = new string[] { ".txt", ".csv" };

                    if (!AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                    {
                        ModelState.AddModelError("File", "Please file of type: " + string.Join(", ", AllowedFileExtensions));
                    }
                    else
                    {
                        //TO:DO -- All successful, save the file to temp location
                        
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/Upload"), fileName);
                        file.SaveAs(path);
                        ModelState.Clear();
                        ViewBag.Message = "File uploaded successfully";
                        string userID = User.Identity.GetUserId();

                        //Write to the LoadedFiles table (FileName, FileType, FileImportDate, UserID)
                         LoadViewModel load = new LoadViewModel();
                         load.loadFileInfo(path, fileName, userID);


                        //LoadViewModel load = new LoadViewModel();
                        //load.loadFile(path, fileName, userID);


                        //Read File


                        // delete the copied file
                        //if (System.IO.File.Exists(path))
                         //   {
                             //   System.IO.File.Delete(path);
                          //  }
                          

                            //return Loaded View passing the table id
                            return Redirect("MyLoads");

                    }

                }
            }
            //Somethings gone wrong, return view
            return View();
        }


        // GET: Load/Process
        public ActionResult Process(Guid id)
        {            

            LoadViewModel load = new LoadViewModel();

            var fileName = load.filename(id);
            var path = Path.Combine(Server.MapPath("~/Content/Upload"), fileName);

            load.loadFile(path, fileName, id.ToString());

            //delete the copied file
            if (System.IO.File.Exists(path))
               {
               System.IO.File.Delete(path);
              }

            //run analysis
           // load.runanalysis(id);

            // once complete, update progress to 100%
            load.progressupdate(id);

            return View();
        }

        // GET: Load/All
        public ActionResult All()
        {

            LoadViewModel load = new LoadViewModel();

            //get list of unprocessed ids
            var ids = load.unprocessedids();

            //for each id run Load/Process/id
            foreach (var id in ids)
            {
                var fileName = load.filename(id);
                var path = Path.Combine(Server.MapPath("~/Content/Upload"), fileName);

                load.loadFile(path, fileName, id.ToString());

                //delete the copied file
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                //run analysis
                // load.runanalysis(id);

                // once complete, update progress to 100%
                load.progressupdate(id);
            }

            return View();
        }

    }
}
