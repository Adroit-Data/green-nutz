using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Data_Inspector.Models;
using Microsoft.AspNet.Identity;

namespace Data_Inspector.Controllers
{
    public class MyLoadsController : Controller
    {
        // GET: MyLoads
        public ActionResult Index()
        {
            using (MyLoadsConnection myLoads = new MyLoadsConnection())
            {
                return View(myLoads.LoadedFiles.ToList().Where(x => x.UserID == User.Identity.GetUserId()));
            }
            
        }

        // GET: MyLoads/View/5
        public ActionResult View(Guid id)
        {
            string ConnStr = ConfigurationManager.ConnectionStrings["LoadedFiles"].ConnectionString;
            var Conn = new SqlConnection(ConnStr);
            string SqlString = "SELECT * FROM ADI_DataInspector.dbo.table_load_" + id.ToString().Replace('-','_');
            SqlDataAdapter sda = new SqlDataAdapter(SqlString, Conn);
            DataTable dt = new DataTable();
            try
            {
                Conn.Open();
                sda.Fill(dt);
            }
            finally
            {
                Conn.Close();
            }
            return View(dt);
           
        }

        // GET: MyLoads/Create
        public ActionResult Create()
        {
            return RedirectToAction("Index", "Load");
        }

        // POST: MyLoads/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
              
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: MyLoads/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: MyLoads/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: MyLoads/Delete/5
        public ActionResult Delete(Guid id)
        {
            using (MyLoadsConnection myLoads = new MyLoadsConnection())
            {
                return View(myLoads.LoadedFiles.Where(x => x.LoadedFileID == id).FirstOrDefault());
            }
        }
        // POST: MyLoads/Delete/5
        [HttpPost]
        public ActionResult Delete(Guid id, FormCollection collection)
        {
            string tableName = null;
            try
            {
                // TODO: Add delete logic here
                using (MyLoadsConnection myLoads = new MyLoadsConnection())
                {
                    LoadedFile loadedfile = myLoads.LoadedFiles.Where(x => x.LoadedFileID == id).FirstOrDefault();
                    myLoads.LoadedFiles.Remove(loadedfile);
                    tableName = loadedfile.LoadedFileID.ToString();
                    myLoads.Database.ExecuteSqlCommand("Drop Table [dbo].table_load_" + tableName.Replace("-", "_"));
                    myLoads.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = "Table not droped";
                return View();
            }
        }
    }
}
