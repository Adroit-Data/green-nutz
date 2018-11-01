using System;
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

                        //Read File
                        using (var streamReader = System.IO.File.OpenText(path))
                        {
                            MySplit split = new MySplit();
                            string source = streamReader.ReadLine();

                            //confirm filetype and detect seperator
                            LoadViewModel LoadView = new LoadViewModel();
                            string fileType = LoadView.DetectFileType(source);
                            char seperator = LoadView.DetectDelimeter(source);

                            //is it a valid structure??
                            if (fileType == "notvalid" || seperator == '?')
                            {
                                ViewBag.Message = "Unsupported File Format.";
                                return View();
                            }

                            //add to table details table, return table id.
                            string loadid;

                            LoadedFiles ctx = new LoadedFiles();
                            LoadedFile loadedfile = new LoadedFile { LoadedFileID = Guid.NewGuid(), FileName = fileName, FileType = fileType, FileImportDate = DateTime.Now, UserID = User.Identity.GetUserId() };
                            ctx.DBLoadedFiles.Add(loadedfile);
                            ctx.SaveChanges();

                            loadid = loadedfile.LoadedFileID.ToString();
                       
                            List<string> fields = split.mySplit(source, seperator);
                            
                            string sql;

                            string sqlproofloadid = loadid.Replace('-', '_');

                            sql = LoadView.GenerateCreateTableSql(fields, sqlproofloadid);

                            using (var newTableCtx = new LoadedFiles())
                            {
                                int noOfTablesCreated = newTableCtx.Database.ExecuteSqlCommand(sql);
                            }


                            while (!streamReader.EndOfStream)
                            {
                                //insert data in to new table
                                source = streamReader.ReadLine();
                                //if (source.Contains("\""))
                                //{

                                //    List<int> openSpeechMarks = new List<int>();
                                //    List<int> closingSpeechMarks = new List<int>();
                                //    List<int> lengthBetweenSpeechMarks = new List<int>();
                                //    List<int> allSpeechMarksPositions = new List<int>();

                                //    for (int i = source.IndexOf('"'); i > -1; i = source.IndexOf('"', i + 1)) // Looping only thru '"'using func IndexOf() Instead of looping through each character to see if it's the one you want
                                //    {
                                //        // for loop end when i=-1 ('"' not found)
                                //        allSpeechMarksPositions.Add(i);

                                //    }

                                //    for (int i = 0; i <= allSpeechMarksPositions.Count()-1; i++)
                                //    {
                                //        if (i % 2 == 0)
                                //        {

                                //            openSpeechMarks.Add(allSpeechMarksPositions[i]);
                                //        }
                                //        else
                                //        {
                                //            closingSpeechMarks.Add(allSpeechMarksPositions[i]);
                                //        }
                                //    }

                                //    for (int i = 0; i <= closingSpeechMarks.Count()-1; i++)
                                //    {
                                //        lengthBetweenSpeechMarks.Add(closingSpeechMarks[i] - openSpeechMarks[i]);
                                //    }

                                //    int firstOne = source.IndexOf("\"", 0);
                                //    int end = source.IndexOf("\"", firstOne + 1);
                                //    int length = end - firstOne;
                                //    List<string> quoutedValues = new List<string>();

                                //    for (int i = 0; i <= lengthBetweenSpeechMarks.Count()-1; i++)
                                //    {
                                //        quoutedValues.Add(source.Substring(openSpeechMarks[i], lengthBetweenSpeechMarks[i]));
                                //    }

                                //    List<string> tempQuoutedValues = new List<string>();
                                //    foreach (string quoutedValue in quoutedValues)
                                //    {
                                //        if (quoutedValue.Contains('.') && quoutedValue.Contains(','))
                                //        {
                                //            tempQuoutedValues.Add(quoutedValue.Replace(",",""));
                                //        }
                                //        else
                                //        {
                                //            tempQuoutedValues.Add(quoutedValue.Replace(",", "-tempComma-"));
                                //        }

                                //    }

                                //    for (int i = 0; i <= tempQuoutedValues.Count() - 1; i++)
                                //    {
                                //        source = source.Replace(quoutedValues[i], tempQuoutedValues[i]);
                                //    }

                                //    source = source.Replace("\"", "");
                                //}                               
                                List<string> values = split.mySplit(source,seperator);
                                //List<string> values2 = new List<string>();
                                //foreach (string value in values)
                                //{
                                    
                                //    values2.Add(value.Replace("-tempComma-", ","));
                                //}

                                sql = LoadView.GenerateInsertInToTableSql(values, sqlproofloadid);

                                //ViewBag.Message = sql;
                                using (var newTableCtx = new LoadedFiles())
                                {
                                     int noOfRecordsInserted = newTableCtx.Database.ExecuteSqlCommand(sql);
                                }
                            }

                            streamReader.Close();

                            // delete the copied file
                            if (System.IO.File.Exists(path))
                            {
                                System.IO.File.Delete(path);
                            }
                            // identifying data types and altering table columns
                            LoadView.SetupColumnsDataTypes(fields, sqlproofloadid);
                    

                            //return Loaded View passing the table id
                            return Redirect("MyLoads");

                        }

                    }
                }
            }
            //Somethings gone wrong, return view
            return View();
        }

    }
}