using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.ConfiguracaoContabil
{
    public class ConfiguracaoContabilController : BaseController
    {
		#region Construtores

		public ConfiguracaoContabilController(Conexao conexao) : base(conexao) { }

		#endregion


        [CustomAuthorize("ConfiguracaoContabil/ConfiguracaoCentroResultado")]
        public async Task<IActionResult> ConfiguracaoCentroResultado()
        {
            return View();
        }


        [CustomAuthorize("ConfiguracaoContabil/ConfiguracaoContaContabil")]
        public async Task<IActionResult> ConfiguracaoContaContabil()
        {
            return View();
        }


        [CustomAuthorize("ConfiguracaoContabil/ConfiguracaoNaturezaOperacao")]
        public async Task<IActionResult> ConfiguracaoNaturezaOperacao()
        {
            return View();
        }

        [CustomAuthorize("ConfiguracaoContabil/ConfiguracaoFechamentoContabilizacao")]
        public async Task<IActionResult> ConfiguracaoFechamentoContabilizacao()
        {
            return View();
        }
    }
}
