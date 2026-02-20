using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Escrituracao
{
	[Area("Relatorios")]
	public class FreteCompetenciaController : Controller
    {
        // GET: Relatorios/FreteCompetencia
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}