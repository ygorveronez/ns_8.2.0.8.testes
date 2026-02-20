using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Produtos
{
	[Area("Relatorios")]
	public class LocalidadesController : Controller
    {
        [CustomAuthorize("Relatorios/Localidades/Localidade")]
        public async Task<IActionResult> Localidade()
        {
            return View();
        }
    }
}