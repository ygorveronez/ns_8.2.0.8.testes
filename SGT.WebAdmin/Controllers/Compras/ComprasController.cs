using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Compras
{
    public class ComprasController : BaseController
    {
		#region Construtores

		public ComprasController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Compras/MotivoCompra")]
        public async Task<IActionResult> MotivoCompra()
        {
            return View();
        }

        [CustomAuthorize("Compras/CondicaoPagamento")]
        public async Task<IActionResult> CondicaoPagamento()
        {
            return View();
        }

        [CustomAuthorize("Compras/RequisicaoMercadoria")]
        public async Task<IActionResult> RequisicaoMercadoria()
        {
            return View();
        }

        [CustomAuthorize("Compras/RegrasRequisicaoMercadoria")]
        public async Task<IActionResult> RegrasRequisicaoMercadoria()
        {
            return View();
        }

        [CustomAuthorize("Compras/CotacaoCompra")]
        public async Task<IActionResult> CotacaoCompra()
        {
            return View();
        }

        [CustomAuthorize("Compras/AutorizacaoRequisicaoMercadoria")]
        public async Task<IActionResult> AutorizacaoRequisicaoMercadoria()
        {
            return View();
        }

        [CustomAuthorize("Compras/OrdemCompra")]
        public async Task<IActionResult> OrdemCompra()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Compras/OrdemCompra");
            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
            return View();
        }

        [CustomAuthorize("Compras/RegrasOrdemCompra")]
        public async Task<IActionResult> RegrasOrdemCompra()
        {
            return View();
        }

        [CustomAuthorize("Compras/AutorizacaoOrdemCompra")]
        public async Task<IActionResult> AutorizacaoOrdemCompra()
        {
            return View();
        }

        [CustomAuthorize("Compras/RespostaCotacao")]
        public async Task<IActionResult> RespostaCotacao()
        {
            return View();
        }

        [CustomAuthorize("Compras/FluxoCompra")]
        public async Task<IActionResult> FluxoCompra()
        {
            return View();
        }
    }
}
