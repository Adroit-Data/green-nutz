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
using LoadingBar;
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

                            Thread t = new Thread(new ThreadStart(new frmMain().StartForm));
                            t.Start();
                            Thread.Sleep(5000);

                            List<string> fields = split.mySplit(source, seperator);

                            string sql;

                            string sqlproofloadid = loadid.Replace('-', '_');

                            sql = LoadView.GenerateCreateTableSql(fields, sqlproofloadid);

                            string ConnStr = ConfigurationManager.ConnectionStrings["LoadedFiles"].ConnectionString;
                            var Conn = new SqlConnection(ConnStr);
                            var CreateTable = new SqlCommand(sql,Conn);
                            Conn.Open();
                            CreateTable.ExecuteNonQuery();

                            var tempDataTable = new DataTable();
                            tempDataTable.Columns.Add(new DataColumn("DIRowID"));
                            for (int i = 0; i < fields.Count; i++)
                            {
                                tempDataTable.Columns.Add(new DataColumn());
                            }

                            
                            while (!streamReader.EndOfStream)
                            {
                                DataRow row = tempDataTable.NewRow();
                                source = streamReader.ReadLine();

                                List<string> values = new List<string>();
                                values.Add(Guid.NewGuid().ToString());
                                values.AddRange(split.mySplit(source, seperator));
                                row.ItemArray = values.ToArray();
                                tempDataTable.Rows.Add(row);

                                //sql = LoadView.GenerateInsertInToTableSql(values, sqlproofloadid);

                                //ViewBag.Message = sql;
                                //using (var newTableCtx = new LoadedFiles())
                                //{
                                //    int noOfRecordsInserted = newTableCtx.Database.ExecuteSqlCommand(sql);
                                //}
                            }

                            streamReader.Close();

                            var bc = new SqlBulkCopy(Conn, SqlBulkCopyOptions.TableLock, null)
                            {
                                DestinationTableName = "table_load_" + sqlproofloadid,
                                BatchSize = tempDataTable.Rows.Count
                            };

                            //Conn.Open();
                            bc.WriteToServer(tempDataTable);
                            Conn.Close();
                            bc.Close();

                            t.Abort();
                            
                                
                         
                      

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