using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdminMultisoftwareApp.Controllers.Usuarios
{
    public class UsuariosController : BaseController
    {
        [CustomAuthorize("Usuarios/Usuario")]
        public ActionResult Usuario()
        {
            return View();
        }
    }
}