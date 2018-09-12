﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Data_Inspector.Models;

namespace Data_Inspector.Controllers
{
    public class MyLoadsController : Controller
    {
        // GET: MyLoads
        public ActionResult Index()
        {
            using (MyLoadsConnection myLoads = new MyLoadsConnection())
            {
                return View(myLoads.LoadedFiles.ToList());
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
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MyLoads/Delete/5
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

    }
}
