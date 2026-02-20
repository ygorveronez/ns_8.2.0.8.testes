using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdminMultisoftwareApp.Controllers.Configuracoes
{
    public class ConfiguracoesController : BaseController
    {
        [CustomAuthorize("Configuracoes/InstanciaBase")]
        public ActionResult InstanciaBase()
        {
            return View();
        }
    }
}