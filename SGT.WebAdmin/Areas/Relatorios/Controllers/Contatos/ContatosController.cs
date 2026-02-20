using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Contatos
{
	[Area("Relatorios")]
	public class ContatosController : Controller
    {
        [CustomAuthorize("Relatorios/Contatos/ContatoCliente")]
        public async Task<IActionResult> ContatoCliente()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Contatos/TipoContatoCliente")]
        public async Task<IActionResult> TipoContatoCliente()
        {
            return View();
        }
        
    }
}