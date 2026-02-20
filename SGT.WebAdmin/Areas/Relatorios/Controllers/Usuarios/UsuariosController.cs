using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Usuarios
{
	[Area("Relatorios")]
	public class UsuariosController : Controller
    {
        [CustomAuthorize("Relatorios/Usuarios/Usuario")]
        public IActionResult Usuario()
        {
            return View();
        }
    }
}