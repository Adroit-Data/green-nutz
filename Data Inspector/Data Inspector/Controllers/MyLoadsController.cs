using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Data_Inspector.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Common;
using Data_Inspector.ViewModels;
using Newtonsoft.Json;


namespace Data_Inspector.Controllers
{
    public class MyLoadsController : Controller
    {
        // GET: MyLoads
        public ActionResult Index()
        {            

            using (MyLoadsConnection myLoads = new MyLoadsConnection())
            {
                

                if (User.IsInRole("Admin"))
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
            ViewBag.TableID = id;
            //Check Data belongs to user.
            using (MyLoadsConnection myLoads = new MyLoadsConnection())
            {
                
                int myUserLoads = myLoads.LoadedFiles.ToList().Where(x => x.UserID == User.Identity.GetUserId()).Where((x => x.LoadedFileID == id)).Count();

                if (User.IsInRole("Admin") || myUserLoads > 0)
                {
                    string ConnStr = ConfigurationManager.ConnectionStrings["LoadedFiles"].ConnectionString;
                    SqlConnection Conn = new SqlConnection(ConnStr);
                    SqlDataAdapter SQLProcedure = new SqlDataAdapter("[dbo].[Sp_GetTableLoadData]", Conn);
                    SQLProcedure.SelectCommand.Parameters.AddWithValue("@Table", id.ToString().Replace('-', '_'));
                    SQLProcedure.SelectCommand.CommandType = CommandType.StoredProcedure;
                    DataTable dt = new DataTable(id.ToString().Replace('-', '_'));//have to pass id as parameter to be able to get table name in the view other wise is just passing table data without actual table name.
                    try
                    {
                        Conn.Open();
                        SQLProcedure.Fill(dt);
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

        public ActionResult Edit(string TableName, string RowId)
        {

            using (MyLoadsConnection myLoads = new MyLoadsConnection())
            {
               
                List<DataForUpdate> dataList = new List<DataForUpdate>();
                //DataForUpdate data = new DataForUpdate();

                string ConnStr = ConfigurationManager.ConnectionStrings["LoadedFiles"].ConnectionString;
                SqlConnection Conn = new SqlConnection(ConnStr);

                SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].table_load_" + TableName + " where DIRowID='" + RowId.ToString() + "'", Conn);

                Conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                //var data = new DataForUpdate[dr.FieldCount];

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {

                        for (int index = 1; index <= dr.FieldCount-1; index++)
                        {                       
                            dataList.Add(new DataForUpdate { columnName = dr.GetName(index), columnValue = dr.GetString(index) });                        
                        }

                    }
                    
                    dr.Close();
                }
                var data = new AllDataForUpdateViewModels
                {
                    tableName = TableName,
                    rowId = RowId.ToString(),
                    DataForUpdate = dataList
                };
                return View(data);
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

                int myUserLoads = myLoads.LoadedFiles.ToList().Where(x => x.UserID == User.Identity.GetUserId()).Where((x => x.LoadedFileID == id)).Count();

                if (User.IsInRole("Admin") || myUserLoads > 0)
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

                int myUserLoads = myLoads.LoadedFiles.ToList().Where(x => x.UserID == User.Identity.GetUserId()).Where((x => x.LoadedFileID == id)).Count();

                if (User.IsInRole("Admin") || myUserLoads > 0)
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

        // POST: MyLoads/update
        [HttpPost]
        public Boolean update(string TableName, string RowId, string ColumnName, string ColumnNewValue) 
        {

            //var TableName = Request["TableName"] ;
            //var RowId = Request["RowId"];
            //var ColumnName = Request["ColumnName"];
            //var ColumnNewValue = Request["ColumnNewValue"];



            int myUserLoads;
            using (MyLoadsConnection myLoads = new MyLoadsConnection())
            {

                string tN = TableName;

                myUserLoads = myLoads.LoadedFiles.ToList().Where(x => x.UserID == User.Identity.GetUserId()).Where((x => x.LoadedFileID.ToString() == tN)).Count();

                

            }
            if (User.IsInRole("Admin") || myUserLoads > 0)
            {

                string ConnStr = ConfigurationManager.ConnectionStrings["LoadedFiles"].ConnectionString;
                using (SqlConnection Conn = new SqlConnection(ConnStr))
                {
                    Conn.Open();

                    using (SqlCommand cmd = new SqlCommand("update [dbo].table_load_" + TableName.ToString().Replace("-", "_") + " set " + ColumnName + " = '"+ColumnNewValue+"' where DIRowID = '" + RowId.ToString() + "'", Conn))
                    {

                        int rows = cmd.ExecuteNonQuery();
                        
                    }
                }
            }

            return true;
        }


        public ContentResult GetTableLoadData(Guid id)
        {

                    string ConnStr = ConfigurationManager.ConnectionStrings["LoadedFiles"].ConnectionString;
                    SqlConnection Conn = new SqlConnection(ConnStr);
                    SqlDataAdapter SQLProcedure = new SqlDataAdapter("[dbo].[Sp_GetTableLoadData]", Conn);
                    SQLProcedure.SelectCommand.Parameters.AddWithValue("@Table", id.ToString().Replace('-', '_'));
                    SQLProcedure.SelectCommand.CommandType = CommandType.StoredProcedure;
                    DataTable dt = new DataTable(id.ToString().Replace('-', '_'));//have to pass id as parameter to be able to get table name in the view other wise is just passing table data without actual table name.
                    Conn.Open();
                    SQLProcedure.Fill(dt);
                    Conn.Close();

            var list = JsonConvert.SerializeObject(dt, Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            });

            return Content(list);

        }

        public ContentResult GetTableLoadHeaders(Guid id)
        {

            string ConnStr = ConfigurationManager.ConnectionStrings["LoadedFiles"].ConnectionString;
            SqlConnection Conn = new SqlConnection(ConnStr);
            SqlDataAdapter SQLProcedure = new SqlDataAdapter("[dbo].[Sp_GetTableLoadHeaders]", Conn);
            SQLProcedure.SelectCommand.Parameters.AddWithValue("@Table", id.ToString().Replace('-', '_'));
            SQLProcedure.SelectCommand.CommandType = CommandType.StoredProcedure;
            DataTable dt = new DataTable(id.ToString().Replace('-', '_'));//have to pass id as parameter to be able to get table name in the view other wise is just passing table data without actual table name.
            Conn.Open();
            SQLProcedure.Fill(dt);
            Conn.Close();

            var list = JsonConvert.SerializeObject(dt, Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            });

            return Content(list);

        }


    }
}
