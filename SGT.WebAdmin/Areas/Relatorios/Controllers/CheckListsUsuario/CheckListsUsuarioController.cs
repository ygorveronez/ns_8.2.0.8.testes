using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers
{
	[Area("Relatorios")]
	public class CheckListsUsuarioController : BaseController
    {
		#region Construtores

		public CheckListsUsuarioController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Relatorios/CheckListsUsuario/CheckListUsuario")]
        public async Task<IActionResult> CheckListUsuario()
        {
            return View();
        }
    }
}
