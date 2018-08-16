using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
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
                                lineposition = lineposition + 1;                               
                                if (lineposition == 1)
                                    //header row ... 1.work out structure 2.create the table                                    
                                {
                                    string line = streamReader.ReadLine();
                                    //does the header row have commas?
                                    int commacount = 0;
                                    foreach (char c in line)
                                        if (c == ',') commacount++;
                                    //does the header row have text qualifiers? i.e. "
                                    int qualifiercount = 0;
                                    foreach (char c in line)
                                        if (c == '"') qualifiercount++;
                                    //work out if it's a csv structure
                                    bool csv = false;
                                    if (2*commacount == qualifiercount || 2 * commacount == 2 + qualifiercount)
                                    {
                                        csv = true;
                                    }
                                    else if(qualifiercount == 0 && commacount > 0)
                                    {
                                        csv = true;
                                    }

                                    //is it a valid structure??
                                    if(csv == false)
                                    {                                        
                                        ViewBag.Message = "Unsupported File Format.";
                                        return View();
                                    }

                                    ViewBag.DataDebug = "Data is " + line + " and there are " + commacount + " commas and " + qualifiercount + " qualifiers!" ;  
                                }
                                else
                                    //data
                                {
                                    var line = streamReader.ReadLine();
                                    var data = line.Split(new[] { ',' });

                                    //dbContext.Persons.Add(person);
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