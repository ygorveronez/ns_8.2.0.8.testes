using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Relatorios
{
    public class RelatoriosRVController : Controller
    {
        [CustomAuthorize("RelatoriosRV/CTesCancelados")]
        public async Task<IActionResult> CTesCancelados()
        {
            return View();
        }

        [CustomAuthorize("RelatoriosRV/CTesEmitidos")]
        public async Task<IActionResult> CTesEmitidos()
        {
            return View();
        }

        [CustomAuthorize("RelatoriosRV/CTesEmitidosEmbarcador")]
        public async Task<IActionResult> CTesEmitidosEmbarcador()
        {
            return View();
        }

        [CustomAuthorize("RelatoriosRV/MDFesEmitidos")]
        public async Task<IActionResult> MDFesEmitidos()
        {
            return View();
        }

        [CustomAuthorize("RelatoriosRV/MDFesNaoEncerrados")]
        public async Task<IActionResult> MDFesNaoEncerrados()
        {
            return View();
        }
    }
}