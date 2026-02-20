using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Justificativa
{
    public class JustificativasController : Controller
    {
        [CustomAuthorize("Justificativas/EncerramentoManualViagem")]
        public async Task<IActionResult> EncerramentoManualViagem()
        {
            return View();
        }
    }
}