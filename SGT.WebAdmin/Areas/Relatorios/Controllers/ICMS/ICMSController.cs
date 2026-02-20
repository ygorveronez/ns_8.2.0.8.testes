using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.ICMS
{
	[Area("Relatorios")]
	public class ICMSController : Controller
    {
        [CustomAuthorize("Relatorios/ICMS/RegraICMS")]
        public async Task<IActionResult> RegraICMS()
        {
            return View();
        }
    }
}