using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdminMultisoftwareApp.Controllers.Modulos
{
    public class ModulosController : BaseController
    {
        [CustomAuthorize("Modulos/Modulo")]
        public ActionResult Modulo()
        {
            return View();
        }

        [CustomAuthorize("Modulos/Formulario")]
        public ActionResult Formulario()
        {
            return View();
        }

        [CustomAuthorize("Modulos/PermissaoPersonalizada")]
        public ActionResult PermissaoPersonalizada()
        {
            return View();
        }

        [CustomAuthorize("Modulos/ClienteModulo")]
        public ActionResult ClienteModulo()
        {
            return View();
        }

        [CustomAuthorize("Modulos/ClienteFormulario")]
        public ActionResult ClienteFormulario()
        {
            return View();
        }

    }
}