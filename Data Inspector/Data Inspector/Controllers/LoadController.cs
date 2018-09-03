using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text.RegularExpressions;
using Data_Inspector.Models;


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
                    int MaxContentLength = 1024 * 1024 * 3; //3 MB
                    string[] AllowedFileExtensions = new string[] { ".txt", ".csv" };

                    if (!AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                    {
                        ModelState.AddModelError("File", "Please file of type: " + string.Join(", ", AllowedFileExtensions));
                    }

                    else if (file.ContentLength > MaxContentLength)
                    {
                        ModelState.AddModelError("File", "Your file is too large, maximum allowed size is: " + MaxContentLength + " MB");
                    }
                    else
                    {
                        //TO:DO -- All successful, save the file to temp location
                        
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/Upload"), fileName);
                        file.SaveAs(path);
                        ModelState.Clear();
                        ViewBag.Message = "File uploaded successfully";

                        //Read File
                        using (var streamReader = System.IO.File.OpenText(path))
                        {
                            int lineposition = 0;
                            //var dbContext = new SampleDbContext();
                            while (!streamReader.EndOfStream)
                            {
                                lineposition++;                               
                                if (lineposition == 1)
                                    //header row ... 1.work out structure 2.create the table                                    
                                {
                                    string headerrow = streamReader.ReadLine();
                                    //detect filetype

                                    string filetype;
                                    string seperator;

                                    LoadViewModel LoadView = new LoadViewModel();                                   
                                    filetype = LoadView.DetectFileType(headerrow);
                                    seperator = LoadView.DetectSeperator(headerrow);

                                    //is it a valid structure??
                                    if (filetype == "notvalid")
                                    {
                                         ViewBag.Message = "Unsupported File Format.";
                                         return View();
                                    }    
                                    //add to table details table, return table id.
                                    string loadid = "0";

                                    //loadid = AddTableViewModel();
                                    
                                    
                                    LoadedFiles ctx = new LoadedFiles();
                                    LoadedFile loadedfile = new LoadedFile { FileName = "test", FileType = "csv" };
                                    ctx.LoadedFiless.Add(loadedfile);
                                    ctx.SaveChanges();                                 

                                    //create table
                                    //turn first line into an array using the now known seperator
                                    string[] fields = Regex.Split(headerrow, seperator); ;

                                    string sql;

                                    sql = LoadView.GenerateCreateTableSql(fields, loadid);

                                    ViewBag.DataDebug = "SQL is " + sql ;  
                                }
                                else
                                    //data
                                {
                                    var line = streamReader.ReadLine();
                                    

                                    
                                }
                            }

                            //dbContext.SaveChanges();
                            //rerun Loaded View passing the table id
                        }

                    }
                }
            }
            //Somethings gone wrong, return view
            return View();
        }

        public ActionResult Loaded(string id)
        {
            // If no id return to Load screen
            if (id == null)
            {
               return RedirectToAction("Index","Load");
            }

            ViewBag.DataFile = "Data Data Data";

            return View();
        }

    }
}