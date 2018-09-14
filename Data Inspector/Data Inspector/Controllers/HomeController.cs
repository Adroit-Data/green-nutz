using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Data_Inspector.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        //make some comment
        public ActionResult Index()
        {
            return View();
        }

    }
}