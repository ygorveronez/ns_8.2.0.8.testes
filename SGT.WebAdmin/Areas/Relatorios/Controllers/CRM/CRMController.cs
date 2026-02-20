using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.CRM
{
	[Area("Relatorios")]
	public class CRMController : BaseController
    {
		#region Construtores

		public CRMController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Relatorios/CRM/Prospeccao")]
        public async Task<IActionResult> Prospeccao()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/CRM/AgendaTarefas")]
        public async Task<IActionResult> AgendaTarefas()
        {
            return View();
        }
    }
}
