using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.PedidosVendas
{
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

        [CustomAuthorize("PedidosVendas/OrdemServicoVenda")]
        public async Task<IActionResult> OrdemServicoVenda()
        {
            return View();
        }

        [CustomAuthorize("PedidosVendas/VendaDireta")]
        public async Task<IActionResult> VendaDireta()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("PedidosVendas/VendaDireta");
            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
            return View();
        }

        [CustomAuthorize("PedidosVendas/TabelaPrecoVenda")]
        public async Task<IActionResult> TabelaPrecoVenda()
        {
            return View();
        }

        [CustomAuthorize("PedidosVendas/OrdemServicoPet")]
        public async Task<IActionResult> OrdemServicoPet()
        {
            return View();
        }
    }
}
