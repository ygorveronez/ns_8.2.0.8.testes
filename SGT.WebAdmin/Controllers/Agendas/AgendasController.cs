using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Agendas
{
    public class AgendasController : BaseController
    {
		#region Construtores

		public AgendasController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Agendas/AgendaTarefa")]
        public async Task<IActionResult> AgendaTarefa()
        {
            return View();
        }

        [CustomAuthorize("Agendas/Agenda")]
        public async Task<IActionResult> Agenda()
        {
            return View();
        }
    }
}
