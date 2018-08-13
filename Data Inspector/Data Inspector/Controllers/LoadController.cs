using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Data_Inspector.Controllers
{
    public class LoadController : Controller
    {
        // GET: Load
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Loaded()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
    }
}