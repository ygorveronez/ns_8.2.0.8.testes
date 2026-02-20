using SGT.WebAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.RH
{
	[Area("Relatorios")]
	public class RHController : BaseController
    {
		#region Construtores

		public RHController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Relatorios/RH/FolhaLancamento")]
        public async Task<IActionResult> FolhaLancamento()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/RH/ComissaoFuncionario")]
        public async Task<IActionResult> ComissaoFuncionario()
        {
            return View();
        }
    }
}
