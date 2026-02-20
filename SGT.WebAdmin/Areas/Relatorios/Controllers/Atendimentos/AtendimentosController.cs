using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Atendimentos
{
	[Area("Relatorios")]
	public class AtendimentosController : BaseController
    {
		#region Construtores

		public AtendimentosController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Atendimentos/Chamado")]
        public async Task<IActionResult> Chamado()
        {
            return View();
        }
    }
}
