using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.OperacoesFiscais
{
    public class OperacoesFiscaisController : Controller
    {
        [CustomAuthorize("OperacoesFiscais/NaturezaOperacao")]
        public async Task<IActionResult> NaturezaOperacao()
        {
            return View();
        }

        [CustomAuthorize("OperacoesFiscais/CFOP")]
        public async Task<IActionResult> CFOP()
        {
            return View();
        }
    }
}