using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Data_Inspector.Models;

namespace Data_Inspector.Controllers
{
    public class UsersListController : Controller
    {
        // GET: UsersList
        public ActionResult Index()
        {
            using(usersConnect usersConnection = new usersConnect())
            {
                return View(usersConnection.AspNetUsers.ToList());
            }
            
        }

        // GET: UsersList/Details/5
        public ActionResult Details(int id)
        {
            return View();
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

        // GET: UsersList/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
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
    }
}
