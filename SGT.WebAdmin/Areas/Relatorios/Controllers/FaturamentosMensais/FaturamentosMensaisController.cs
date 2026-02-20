using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.FaturamentosMensais
{
	[Area("Relatorios")]
	public class FaturamentosMensaisController : Controller
    {
        // GET: Relatorios/FaturamentosMensais
        public async Task<IActionResult> CobrancaMensal()
        {
            return View();
        }
    }
}