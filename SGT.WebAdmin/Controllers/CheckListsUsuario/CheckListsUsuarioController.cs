using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Checklists
{
    public class CheckListsUsuarioController : BaseController
    {
		#region Construtores

		public CheckListsUsuarioController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("CheckListsUsuario/ConfigCheckListUsuario")]
        public async Task<IActionResult> ConfigCheckListUsuario()
        {
            return View();
        }

        [CustomAuthorize("CheckListsUsuario/CheckListUsuario")]
        public async Task<IActionResult> ChecklistUsuario()
        {
            return View();
        }
    }
}
