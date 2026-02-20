using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Auditoria
{
	[Area("Relatorios")]
	public class AuditoriaController : Controller
    {
        [CustomAuthorize("Relatorios/Auditoria/Auditoria")]
        public async Task<IActionResult> Auditoria()
        {
            return View();
        }
    }
}