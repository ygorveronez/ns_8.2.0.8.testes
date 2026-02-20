using SGT.WebAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Compras
{
	[Area("Relatorios")]
	public class ComprasController : BaseController
    {
		#region Construtores

		public ComprasController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Relatorios/Compras/OrdemCompra")]
        public async Task<IActionResult> OrdemCompra()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Compras/PontuacaoComprador")]
        public async Task<IActionResult> PontuacaoComprador()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Compras/NotaEntradaOrdemCompra")]
        public async Task<IActionResult> NotaEntradaOrdemCompra()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Compras/PontuacaoFornecedor")]
        public async Task<IActionResult> PontuacaoFornecedor()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Compras/SugestaoCompra")]
        public async Task<IActionResult> SugestaoCompra()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Compras/RequisicaoMercadoria")]
        public async Task<IActionResult> RequisicaoMercadoria()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Compras/CotacaoCompra")]
        public async Task<IActionResult> CotacaoCompra()
        {
            return View();
        }
    }
}
