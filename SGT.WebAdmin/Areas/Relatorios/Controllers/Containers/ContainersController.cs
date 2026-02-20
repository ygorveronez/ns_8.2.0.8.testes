using SGT.WebAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.ConfiguracaoContabil
{
	[Area("Relatorios")]
	public class ContainersController : BaseController
    {
		#region Construtores

		public ContainersController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Relatorios/Containers/ControleContainer")]
        public async Task<IActionResult> ControleContainer()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Containers/HistoricoMovimentacaoContainers")]
        public async Task<IActionResult> HistoricoMovimentacaoContainers()
        {
            return View();
        }
    }
}
