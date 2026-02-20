using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cotacoes
{
    public class CotacoesController : BaseController
    {
		#region Construtores

		public CotacoesController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Cotacoes/CotacaoPedido")]
        public async Task<IActionResult> CotacaoPedido()
        {
            return View();
        }

        [CustomAuthorize("Cotacoes/RegraCotacaoPedido")]
        public async Task<IActionResult> RegraCotacaoPedido()
        {
            return View();
        }

        [CustomAuthorize("Cotacoes/AutorizacaoCotacaoPedido")]
        public async Task<IActionResult> AutorizacaoCotacaoPedido()
        {
            return View();
        }

        [CustomAuthorize("Cotacoes/RegraCotacao")]
        public async Task<IActionResult> RegraCotacao()
        {
            return View();
        }

        [CustomAuthorize("Cotacoes/CotacaoFrete")]
        public async Task<IActionResult> CotacaoFrete()
        {
            return View();
        }
    }
}
