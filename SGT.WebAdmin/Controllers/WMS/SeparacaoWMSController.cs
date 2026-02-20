using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.WMS
{
    public class SeparacaoWMSController : Controller
    {
        // GET: SeparacaoWMS
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}