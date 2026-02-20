using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdminMultisoftwareApp.Controllers.Home
{
    public class HomeController : BaseController
    {
        [CustomAuthorize("Home")]
        public ActionResult Index()
        {
            return View();
        }
    }
}