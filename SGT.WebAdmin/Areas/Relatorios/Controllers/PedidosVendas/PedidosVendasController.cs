using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.PedidosVendas
{
	[Area("Relatorios")]
	public class PedidosVendasController : BaseController
    {
		#region Construtores

		public PedidosVendasController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("PedidosVendas/PedidoVenda")]
        public async Task<IActionResult> PedidoVenda()
        {
            return View();
        }

        [CustomAuthorize("PedidosVendas/VendaDireta")]
        public async Task<IActionResult> VendaDireta()
        {
            return View();
        }
    }
}
