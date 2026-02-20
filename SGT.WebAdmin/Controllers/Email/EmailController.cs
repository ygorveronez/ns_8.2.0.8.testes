using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Email
{
    public class EmailController : BaseController
    {
		#region Construtores

		public EmailController(Conexao conexao) : base(conexao) { }

		#endregion

         [CustomAuthorize("Email/ConfigEmailDocTransporte")]
        public async Task<IActionResult> ConfigEmailDocTransporte()
        {
            return View();
        }

        [CustomAuthorize("Email/EmailDocumentacaoCarga")]
        public async Task<IActionResult> EmailDocumentacaoCarga()
        {
            return View();
        }

        [CustomAuthorize("Email/EmailGlobalizadoFornecedor")]
        public async Task<IActionResult> EmailGlobalizadoFornecedor()
        {
            return View();
        }
    }
}
