using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.BusinessIntelligence.Controllers.Pallets
{
	[Area("BusinessIntelligence")]
	public class PalletsController : Controller
    {
        [CustomAuthorize("BusinessIntelligence/Pallets/QuantidadePallets")]
        public async Task<IActionResult> QuantidadePallets()
        {
            return View();
        }
    }
}