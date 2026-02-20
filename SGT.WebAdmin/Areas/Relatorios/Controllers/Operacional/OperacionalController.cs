using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Operacional
{
	[Area("Relatorios")]
	public class OperacionalController : BaseController
    {
		#region Construtores

		public OperacionalController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Relatorios/Operacional/ConfiguracaoOperadores")]
        public async Task<IActionResult> ConfiguracaoOperadores()
        {
            return View();
        }

    }
}
