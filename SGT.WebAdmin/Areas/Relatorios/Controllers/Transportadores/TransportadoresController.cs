using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Transportadores
{
	[Area("Relatorios")]
	public class TransportadoresController : Controller
    {
        [CustomAuthorize("Relatorios/Transportadores/Transportadores")]
        public IActionResult Transportadores()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Transportadores/VencimentoCertificado")]
        public IActionResult VencimentoCertificado()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Transportadores/AceiteContrato")]
        public IActionResult AceiteContrato()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Transportadores/ConfiguracoesNFSe")]
        public IActionResult ConfiguracoesNFSe()
        {
            return View();
        }
    }
}