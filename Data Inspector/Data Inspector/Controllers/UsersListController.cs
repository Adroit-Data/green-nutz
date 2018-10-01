using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Web.Mvc;
using Data_Inspector.Models;
using Microsoft.AspNet.Identity;
using System.Data.SqlClient;
using System.Data;

namespace Data_Inspector.Controllers
{
    public class UsersListController : Controller
    {
        // GET: UsersList
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                using (usersConnect usersConnection = new usersConnect())
                {
                    return View(usersConnection.AspNetUsers.ToList());
                }
            }

            return RedirectToAction("NoAuth", "MyLoads");

        }

        // GET: UsersList/Details/5
        public ActionResult Details(string id)
        {
            if (User.IsInRole("Admin"))
            {
                using (usersConnect usersConnection = new usersConnect())
                {
                    return View(usersConnection.AspNetUsers.Where(x => x.Id == id).FirstOrDefault());
                }
            }

            return RedirectToAction("NoAuth", "MyLoads");
        }

        // GET: UsersList/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UsersList/Create
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
        // GET: UsersList/UploadedFiles/5
        public ActionResult UploadedFiles(string id)
        {
            if (User.IsInRole("Admin"))
            {
                using (MyLoadsConnection usersConnection = new MyLoadsConnection())
                {
                    return View(usersConnection.LoadedFiles.Where(x => x.UserID == id).ToList());
                }
            }
            return View();
        }
        // GET: UsersList/UploadedFiles/5
        public ActionResult View(Guid id)
        {
            using (MyLoadsConnection myLoads = new MyLoadsConnection())
            {
                int myUserLoads = myLoads.LoadedFiles.ToList().Where(x => x.UserID == User.Identity.GetUserId()).Where((x => x.LoadedFileID == id)).Count();

                if (User.IsInRole("Admin") || myUserLoads > 0)
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
    
    

        // POST: UsersList/Edit/5
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

        // GET: UsersList/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UsersList/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public JsonResult GetUsers()
        {
            usersConnect e = new usersConnect();
            var result = e.AspNetUsers.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);

        }
    }
}
