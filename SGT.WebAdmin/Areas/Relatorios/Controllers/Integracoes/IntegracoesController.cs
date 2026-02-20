using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Integracoes
{
	[Area("Relatorios")]
	public class IntegracoesController : BaseController
    {
		#region Construtores

		public IntegracoesController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Relatorios/Integracoes/IndicadorIntegracaoCTe")]
        public async Task<IActionResult> IndicadorIntegracaoCTe()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Integracoes/PedidoAguardandoIntegracao")]
        public async Task<IActionResult> PedidoAguardandoIntegracao()
        {
            return View();
        }
    }
}

