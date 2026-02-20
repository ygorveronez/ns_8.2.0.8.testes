using SGT.WebAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Pedidos
{
	[Area("Relatorios")]
	public class PedidosController : BaseController
    {
		#region Construtores

		public PedidosController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Relatorios/Pedidos/PedidoOcorrencia")]
        public async Task<IActionResult> PedidoOcorrencia()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Pedidos/PedidoDevolucao")]
        public async Task<IActionResult> PedidoDevolucao()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Pedidos/Booking")]
        public async Task<IActionResult> Booking()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Pedidos/PedidoRetornoOcorrencia")]
        public async Task<IActionResult> PedidoRetornoOcorrencia()
        {
            return View();
        }
    }
}
