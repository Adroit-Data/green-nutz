﻿using System.Web;
using System.Web.Mvc;

namespace Data_Inspector
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
