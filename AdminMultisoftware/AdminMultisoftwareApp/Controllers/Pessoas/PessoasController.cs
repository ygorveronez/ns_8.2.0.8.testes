using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdminMultisoftwareApp.Controllers.Pessoas
{
    public class PessoasController : BaseController
    {
        [CustomAuthorize("Pessoas/ClienteURLAcesso")]
        public ActionResult ClienteURLAcesso()
        {
            return View();
        }

        [CustomAuthorize("Pessoas/Cliente")]
        public ActionResult Cliente()
        {
            return View();
        }
    }
}