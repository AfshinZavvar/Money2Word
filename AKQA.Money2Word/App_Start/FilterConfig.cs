﻿using System.Web;
using System.Web.Mvc;

namespace AKQA.Money2Word
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
