using AzureFileStorage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Avbox.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/Index
        public ActionResult Index()
        {
            return View();
        }
    }
}
