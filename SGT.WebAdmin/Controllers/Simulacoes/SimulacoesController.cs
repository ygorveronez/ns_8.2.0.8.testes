using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Simulacoes
{
    public class SimulacoesController : BaseController
    {
		#region Construtores

		public SimulacoesController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Simulacoes/RegrasAutorizacaoSimulacao")]
        public async Task<IActionResult> RegrasAutorizacaoSimulacao()
        {
            return View();
        }
        
        [CustomAuthorize("Simulacoes/GrupoBonificacao")]
        public async Task<IActionResult> GrupoBonificacao()
        {
            return View();
        }
    }
}
