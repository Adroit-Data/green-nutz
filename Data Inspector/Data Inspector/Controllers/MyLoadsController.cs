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
using Microsoft.AspNet.Identity.Owin;

namespace Data_Inspector.Controllers
{
    public class MyLoadsController : Controller
    {
        // GET: MyLoads
        public ActionResult Index()
        {
            using (MyLoadsConnection myLoads = new MyLoadsConnection())
            {
                var userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var roles = userManager.GetRoles(User.Identity.GetUserId());
                if (roles.Contains("Admin"))
                {
                    return View(myLoads.LoadedFiles.ToList());
                }
                else
                {
                    return View(myLoads.LoadedFiles.ToList().Where(x => x.UserID == User.Identity.GetUserId()));
                }
            }
            
        }

        // GET: MyLoads/NoAuth
        public ActionResult NoAuth()
        {
            return View();

        }

        // GET: MyLoads/View/5
        public ActionResult View(Guid id)
        {
            //Check Data belongs to user.
            using (MyLoadsConnection myLoads = new MyLoadsConnection())
            {
                var userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var roles = userManager.GetRoles(User.Identity.GetUserId());

                int myUserLoads = myLoads.LoadedFiles.ToList().Where(x => x.UserID == User.Identity.GetUserId()).Where((x => x.LoadedFileID == id)).Count();

                if (roles.Contains("Admin")|| myUserLoads > 0)
                {
                    string ConnStr = ConfigurationManager.ConnectionStrings["LoadedFiles"].ConnectionString;
                    var Conn = new SqlConnection(ConnStr);
                    string SqlString = "SELECT * FROM ADI_DataInspector.dbo.table_load_" + id.ToString().Replace('-', '_');
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
               
                

                return RedirectToAction("NoAuth", "MyLoads");

            }
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


        // GET: MyLoads/Delete/5
        public ActionResult Delete(Guid id)
        {
            using (MyLoadsConnection myLoads = new MyLoadsConnection())
            {
                var userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var roles = userManager.GetRoles(User.Identity.GetUserId());

                int myUserLoads = myLoads.LoadedFiles.ToList().Where(x => x.UserID == User.Identity.GetUserId()).Where((x => x.LoadedFileID == id)).Count();

                if (roles.Contains("Admin") || myUserLoads > 0)
                {
                    
                return View(myLoads.LoadedFiles.Where(x => x.LoadedFileID == id).FirstOrDefault());
                    
                }
            }

            return RedirectToAction("Index");

        }
        // POST: MyLoads/Delete/5
        [HttpPost]
        public ActionResult Delete(Guid id, FormCollection collection)
        {

            using (MyLoadsConnection myLoads = new MyLoadsConnection())
            {
                var userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var roles = userManager.GetRoles(User.Identity.GetUserId());

                int myUserLoads = myLoads.LoadedFiles.ToList().Where(x => x.UserID == User.Identity.GetUserId()).Where((x => x.LoadedFileID == id)).Count();

                if (roles.Contains("Admin") || myUserLoads > 0)
                {
                    string tableName = null;
                    try
                    {
                        // TODO: Add delete logic here

                        LoadedFile loadedfile = myLoads.LoadedFiles.Where(x => x.LoadedFileID == id).FirstOrDefault();
                        myLoads.LoadedFiles.Remove(loadedfile);
                        tableName = loadedfile.LoadedFileID.ToString();
                        myLoads.Database.ExecuteSqlCommand("Drop Table [dbo].table_load_" + tableName.Replace("-", "_"));
                        myLoads.SaveChanges();

                        return RedirectToAction("Index");
                    }
                    catch
                    {
                        ViewBag.Message = "Table not droped";
                        return View();
                    }
                }
            }


            return RedirectToAction("Index");

        }
    }
}
