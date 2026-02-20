using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Painel
{
    public class PainelController : Controller
    {
        [CustomAuthorize("Painel/Carregamentos")]
        public async Task<IActionResult> Carregamentos()
        {
            return View();
        }

        [CustomAuthorize("Painel/Indicador")]
        public async Task<IActionResult> Indicador()
        {
            return View();
        }
    }
}