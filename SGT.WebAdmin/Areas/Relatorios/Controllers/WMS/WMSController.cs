using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.WMS
{
	[Area("Relatorios")]
	public class WMSController : Controller
    {
        [CustomAuthorize("Relatorios/WMS/ConferenciaVolume")]
        public async Task<IActionResult> ConferenciaVolume()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/WMS/ExpedicaoVolume")]
        public async Task<IActionResult> ExpedicaoVolume()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/WMS/SaldoArmazenamento")]
        public async Task<IActionResult> SaldoArmazenamento()
        {
            return View();
        }       
        
        [CustomAuthorize("Relatorios/WMS/Armazenagem")]
        public async Task<IActionResult> Armazenagem()
        {
            return View();
        }


        [CustomAuthorize("Relatorios/WMS/RastreabilidadeVolumes")]
        public async Task<IActionResult> RastreabilidadeVolumes()
        {
            return View();
        }
    }
}