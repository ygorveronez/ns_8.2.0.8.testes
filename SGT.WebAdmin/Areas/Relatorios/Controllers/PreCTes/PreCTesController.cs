using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.PreCTes
{
    [Area("Relatorios")]
    public class PreCTesController : Controller
    {
        [CustomAuthorize("Relatorios/PreCTes/PreCTe")]
        public async Task<IActionResult> PreCTe()
        {
            return View();
        }
    }
}