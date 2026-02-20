using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.AtendimentoCRM
{
    public class AtendimentosCRMController : BaseController
    {
		#region Construtores

		public AtendimentosCRMController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("AtendimentosCRM/AtendimentoCRM")]
        public async Task<IActionResult> AtendimentoCRM()
        {
            return View();
        }
    }
}
