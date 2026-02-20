using SGT.WebAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Escrituracao
{
	[Area("Relatorios")]
	public class EscrituracaoController : BaseController
    {
		#region Construtores

		public EscrituracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Relatorios/Escrituracao/FreteContabil")]
        public async Task<IActionResult> FreteContabil()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Escrituracao/Competencia")]
        public async Task<IActionResult> Competencia()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Escrituracao/SaldoProvisao")]
        public async Task<IActionResult> SaldoProvisao()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Escrituracao/IntegracaoLotePagamento")]
        public async Task<IActionResult> IntegracaoLotePagamento()
        {
            return View();
        }
    }
}
