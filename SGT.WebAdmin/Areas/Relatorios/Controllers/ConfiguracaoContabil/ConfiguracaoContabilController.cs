using SGT.WebAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.ConfiguracaoContabil
{
	[Area("Relatorios")]
	public class ConfiguracaoContabilController : BaseController
    {
		#region Construtores

		public ConfiguracaoContabilController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Relatorios/ConfiguracaoContabil/ConfiguracaoCentroResultado")]
        public async Task<IActionResult> ConfiguracaoCentroResultado()
        {
            return View();
        }
    }
}
